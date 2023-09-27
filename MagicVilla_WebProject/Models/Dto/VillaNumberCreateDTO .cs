using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebProject.Models.Dto;

public class VillaNumberCreateDTO
{
    [Required]
    public int VillaNo { get; set; }
    //[Required]
    //public int VillaID { get; set; }
    [Required]
    public int VillaId { get; set; }
    public string SpecialDetails { get; set; }
}

