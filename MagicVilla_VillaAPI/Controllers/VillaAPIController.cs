using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly ILogger<VillaAPIController> _logger;
    //private readonly ApplicationDbContext _db;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;

    //private readonly ILogging _logger;


    public VillaAPIController(ILogger<VillaAPIController> logger /*ILogging logger, ApplicationDbContext db*/,IVillaRepository dbVilla, IMapper mapper)
    {
        //_logger = logger;
        _logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        _logger.LogInformation("Get All Villas", "");

        IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
        //to convert villaList to VillaDTO using automapper
        return Ok(_mapper.Map<List<VillaDTO>>(villaList));
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    //[ProducesResponseType( 200, Type = typeof(VillaDTO))]
    //[ProducesResponseType( 400)]
    //[ProducesResponseType( 404)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {

        if (id == 0)
        {
            _logger.LogError("Get Villa Error with Id" + id /*"error"*/);
            return BadRequest();
        }
        var villa = await _dbVilla.GetAsync(u => u.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<VillaDTO>(villa));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDTO) 
    {
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState);
        //}
        //custom validation
        if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
        {
            //key moze biti prazan
            ModelState.AddModelError("CustomError", "Villa already Exists!");
            return BadRequest(ModelState);
        }

        if (createDTO == null)
        {
            return BadRequest(createDTO);
        }
        //if (villaDTO.Id > 0)
        //{
        //    return StatusCode(StatusCodes.Status500InternalServerError);
        //}
        //vilaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id +1;
        
        Villa model = _mapper.Map<Villa>(createDTO);
        //ne trebamo da mappiramo samostalno model to dto, zato sto koristimo automapper
        //Villa model = new()
        //{
        //    Amenity = createDTO.Amenity,
        //    Details = createDTO.Details,
        //    ImageUrl = createDTO.ImageUrl,
        //    Name = createDTO.Name,
        //    Occupancy = createDTO.Occupancy,
        //    Rate = createDTO.Rate,
        //    Sqft = createDTO.Sqft
        //};
        
        await _dbVilla.CreateAsync(model);
        //VillaStore.villaList.Add(vilaDTO);

        return CreatedAtRoute("GetVilla", new { id = model.Id }, model); 
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        //VillaStore.villaList.Remove(villa);
        await _dbVilla.RemoveAsync(villa);
        //_db.Villas.Remove(villa);
        //204
        return NoContent(); //return Ok();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaDTO updateDTO)
    {
        if (updateDTO == null || id != updateDTO.Id)
        {
            return BadRequest();
        }

        //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        //villa.Name = villaDTO.Name;
        //villa.Sqft = villaDTO.Sqft;
        //villa.Occupancy = villaDTO.Occupancy;
        
        //ne trebamo da mappiramo samostalno model to dto, zato sto koristimo automapper
        Villa model = _mapper.Map<Villa>(updateDTO);
        //Villa model = new()
        //{
        //    Amenity = updateDTO.Amenity,
        //    Details = updateDTO.Details,
        //    Id = updateDTO.Id,
        //    ImageUrl = updateDTO.ImageUrl,
        //    Name = updateDTO.Name,
        //    Occupancy = updateDTO.Occupancy,
        //    Rate = updateDTO.Rate,
        //    Sqft = updateDTO.Sqft
        //};
        await _dbVilla.UpdateAsync(model);
        

        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
        {
            return BadRequest();
        }

        // moramo dodati AsNoTracking() jer ce EF track dva Id (VillaDTO i Villa) a to ne moze
        var  villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

        VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
        //ne trebamo da mappiramo samostalno model to dto, zato sto koristimo automapper
        //VillaUpdateDTO villaDTO = new()
        //{
        //    Amenity = villa.Amenity,
        //    Details = villa.Details,
        //    Id = villa.Id,
        //    ImageUrl = villa.ImageUrl,
        //    Name = villa.Name,
        //    Occupancy = villa.Occupancy,
        //    Rate = villa.Rate,
        //    Sqft = villa.Sqft
        //};

        if (villa == null)
        {
            return BadRequest();
        }

        patchDTO.ApplyTo(villaDTO, ModelState);

        Villa model = _mapper.Map<Villa>(villaDTO);
        //Villa model = new()
        //{
        //    Amenity = villaDTO.Amenity,
        //    Details = villaDTO.Details,
        //    Id = villaDTO.Id,
        //    ImageUrl = villaDTO.ImageUrl,
        //    Name = villaDTO.Name,
        //    Occupancy = villaDTO.Occupancy,
        //    Rate = villaDTO.Rate,
        //    Sqft = villaDTO.Sqft
        //};

        await _dbVilla.UpdateAsync(model);
      
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return NoContent();
    }
}
