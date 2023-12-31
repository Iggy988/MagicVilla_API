﻿using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        //mapping source to destination
        CreateMap<Villa, VillaDTO>();
        CreateMap<VillaDTO, Villa>();
        //ReverseMap - za mappiranje reverse
        CreateMap<Villa, VillaCreateDTO>().ReverseMap();
        CreateMap<Villa, VillaUpdateDTO>().ReverseMap();


        CreateMap<VillaNumber, VillaNumberDTO>();
        CreateMap<VillaNumberDTO, VillaNumber>();

        CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
    }
}


