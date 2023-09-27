using AutoMapper;
using MagicVilla_WebProject.Models.Dto;

namespace MagicVilla_WebProject;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        //mapping source to destination
        CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
        CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();

        CreateMap<VillaNumberDTO, VillaNumberCreateDTO>().ReverseMap();
        CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
        
      
    }
}


