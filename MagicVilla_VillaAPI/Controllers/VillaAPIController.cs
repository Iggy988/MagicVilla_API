using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly ILogger<VillaAPIController> _logger;
    //private readonly ApplicationDbContext _db;
    protected APIResponse _response;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;

    //private readonly ILogging _logger;


    public VillaAPIController(ILogger<VillaAPIController> logger /*ILogging logger, ApplicationDbContext db*/,IVillaRepository dbVilla, IMapper mapper)
    {
        //_logger = logger;
        _logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
        this._response = new APIResponse();
    }


    [HttpGet]
    //public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    public async Task<ActionResult<APIResponse>> GetVillas()
    {
        _logger.LogInformation("Get All Villas", "");

        try
        {
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
            _response.StatusCode = HttpStatusCode.OK;
            //to convert villaList to VillaDTO using automapper
            //return Ok(_mapper.Map<List<VillaDTO>>(villaList));
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString()};
            
        }
        return _response;
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    //[ProducesResponseType( 200, Type = typeof(VillaDTO))]
    //[ProducesResponseType( 400)]
    //[ProducesResponseType( 404)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                _logger.LogError("Get Villa Error with Id" + id /*"error"*/);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
            //return Ok(_mapper.Map<VillaDTO>(villa));
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
            
        }
        return _response;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody]VillaCreateDTO createDTO) 
    {
        try
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

            Villa villa = _mapper.Map<Villa>(createDTO);
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

            await _dbVilla.CreateAsync(villa);
            //VillaStore.villaList.Add(vilaDTO);

            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
        }
        
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString()};
            
        }
        return _response;
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        try
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
            //return NoContent(); //return Ok();
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };

        }
        return _response;

    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaDTO updateDTO)
    {
        try
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

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
            //return NoContent();
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };

        }
        return _response;

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
