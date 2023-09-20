using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
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
    private readonly ApplicationDbContext _db;

    //private readonly ILogging _logger;


    public VillaAPIController(ILogger<VillaAPIController> logger /*ILogging logger*/, ApplicationDbContext db)
    {
        //_logger = logger;
        _logger = logger;
        _db = db;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        _logger.LogInformation("Get All Villas", "");
        return Ok(await _db.Villas.ToListAsync());
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
        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO villaDTO) 
    {
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState);
        //}
        //custom validation
        if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
        {
            //key moze biti prazan
            ModelState.AddModelError("CustomError", "Villa already Exists!");
            return BadRequest(ModelState);
        }

        if (villaDTO == null)
        {
            return BadRequest(villaDTO);
        }
        //if (villaDTO.Id > 0)
        //{
        //    return StatusCode(StatusCodes.Status500InternalServerError);
        //}
        //vilaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id +1;
        Villa model = new()
        {
            Amenity = villaDTO.Amenity,
            Details = villaDTO.Details,
            ImageUrl = villaDTO.ImageUrl,
            Name = villaDTO.Name,
            Occupancy = villaDTO.Occupancy,
            Rate = villaDTO.Rate,
            Sqft = villaDTO.Sqft
        };
        
        await _db.Villas.AddAsync(model);
        await _db.SaveChangesAsync();
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

        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        //VillaStore.villaList.Remove(villa);
        _db.Villas.Remove(villa);
        await _db.SaveChangesAsync();
        //204
        return NoContent(); //return Ok();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
    {
        if (villaDTO == null || id != villaDTO.Id)
        {
            return BadRequest();
        }

        //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        //villa.Name = villaDTO.Name;
        //villa.Sqft = villaDTO.Sqft;
        //villa.Occupancy = villaDTO.Occupancy;

        Villa model = new()
        {
            Amenity = villaDTO.Amenity,
            Details = villaDTO.Details,
            Id = villaDTO.Id,
            ImageUrl = villaDTO.ImageUrl,
            Name = villaDTO.Name,
            Occupancy = villaDTO.Occupancy,
            Rate = villaDTO.Rate,
            Sqft = villaDTO.Sqft
        };
        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

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
        var  villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);


        VillaUpdateDTO villaDTO = new()
        {
            Amenity = villa.Amenity,
            Details = villa.Details,
            Id = villa.Id,
            ImageUrl = villa.ImageUrl,
            Name = villa.Name,
            Occupancy = villa.Occupancy,
            Rate = villa.Rate,
            Sqft = villa.Sqft
        };

        if (villa == null)
        {
            return BadRequest();
        }

        patchDTO.ApplyTo(villaDTO, ModelState);

        Villa model = new()
        {
            Amenity = villaDTO.Amenity,
            Details = villaDTO.Details,
            Id = villaDTO.Id,
            ImageUrl = villaDTO.ImageUrl,
            Name = villaDTO.Name,
            Occupancy = villaDTO.Occupancy,
            Rate = villaDTO.Rate,
            Sqft = villaDTO.Sqft
        };

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();


        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return NoContent();
    }
}
