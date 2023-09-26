using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models;

public class VillaNumber
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int VillaNo { get; set; }
    //moramo izbrisati sve unose iz VillaNumber tabele prije database-update
    [ForeignKey("Villa")] //refernciramo prop => public Villa Villa { get; set; }
    public int VillaId { get; set; }
    public Villa Villa { get; set; }
    public string SpecialDetails { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
