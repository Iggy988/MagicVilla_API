using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    //private readonly ILogger<VillaAPIController> _logger;
    private readonly ILogging _logger;

    public VillaAPIController(/*ILogger<VillaAPIController> logger*/ ILogging logger)
    {
        //_logger = logger;
        _logger = logger;
    }


    [HttpGet]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
        _logger.Log("Get All Villas", "");
        return Ok(VillaStore.villaList);
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    //[ProducesResponseType( 200, Type = typeof(VillaDTO))]
    //[ProducesResponseType( 400)]
    //[ProducesResponseType( 404)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> GetVilla(int id)
    {

        if (id == 0)
        {
            _logger.Log("Get Villa Error with Id" + id, "error");
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
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
    public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO vilaDTO) 
    {
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState);
        //}
        //custom validation
        if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == vilaDTO.Name.ToLower()) != null)
        {
            //key moze biti prazan
            ModelState.AddModelError("CustomError", "Villa already Exists!");
            return BadRequest(ModelState);
        }

        if (vilaDTO == null)
        {
            return BadRequest(vilaDTO);
        }
        if (vilaDTO.Id > 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        vilaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id +1;
        VillaStore.villaList.Add(vilaDTO);

        return CreatedAtRoute("GetVilla", new { id = vilaDTO.Id }, vilaDTO); 
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        VillaStore.villaList.Remove(villa);
        //204
        return NoContent(); //return Ok();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
    {
        if (villaDTO == null || id != villaDTO.Id)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        villa.Name = villaDTO.Name;
        villa.Sqft = villaDTO.Sqft;
        villa.Occupancy = villaDTO.Occupancy;

        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        if (villa == null)
        {
            return BadRequest();
        }

        patchDTO.ApplyTo(villa, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return NoContent();
    }
}
