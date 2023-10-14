using MagicVilla_WebProject.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_WebProject.Models.ViewModel;

public class VillaNumberUpdateVM
{
    public VillaNumberUpdateVM()
    {
        VillaNumber = new VillaNumberUpdateDTO();
    }

    public VillaNumberUpdateDTO VillaNumber { get; set; }
    
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }
}
