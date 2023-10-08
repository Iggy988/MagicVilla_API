﻿using AutoMapper;
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

[Route("api/VillaNumberAPI")]
[ApiController]
public class VillaNumberAPIController : ControllerBase
{
    protected APIResponse _response;
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;



    public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
    {

        _dbVillaNumber = dbVillaNumber;
        _mapper = mapper;
        this._response = new APIResponse();
        _dbVilla = dbVilla;
    }


    [HttpGet]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {

        try
        {
            //mora veliko slovo Villa posto je iz VillaNumber element
            IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa"); 
            _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
            _response.StatusCode = HttpStatusCode.OK;
      
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString()};
            
        }
        return _response;
    }

    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
            if (villaNumber == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody]VillaNumberCreateDTO createDTO) 
    {
        try
        {
            
            if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Number already Exists!");
                return BadRequest(ModelState);
            }

            if (await _dbVilla.GetAsync(u => u.Id == createDTO.VillaId) == null)
            {
                ModelState.AddModelError("CustomError", "Villa ID is invalid");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }
           

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);


            await _dbVillaNumber.CreateAsync(villaNumber);

            _response.Result = _mapper.Map<VillaDTO>(villaNumber);
            _response.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _response);
        }
        
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString()};
            
        }
        return _response;
    }

    [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
            if (villaNumber == null)
            {
                return NotFound();
            }
            //VillaStore.villaList.Remove(villa);
            await _dbVillaNumber.RemoveAsync(villaNumber);
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

    [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody]VillaNumberUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.VillaNo)
            {
                return BadRequest();
            }

            if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaId) == null)
            {
                ModelState.AddModelError("CustomError", "Villa ID is invalid");
                return BadRequest(ModelState);
            }

            //ne trebamo da mappiramo samostalno model to dto, zato sto koristimo automapper
            VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

            await _dbVillaNumber.UpdateAsync(model);

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
}
