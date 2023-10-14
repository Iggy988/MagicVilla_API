using MagicVilla_WebProject.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_WebProject.Models.ViewModel;

public class VillaNumberDeleteVM
{
    public VillaNumberDeleteVM()
    {
        VillaNumber = new VillaNumberDTO();
    }

    public VillaNumberDTO VillaNumber { get; set; }
    
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }
}
