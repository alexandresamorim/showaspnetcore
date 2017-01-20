using System.ComponentModel.DataAnnotations;

namespace Showaspnetcore.Models.Enums
{
    public enum ResultadoStatusEnumView
    {
        [Display(Name = "Pendente")]
        Pendente = 1,
        [Display(Name = "Liberado")]
        Liberado = 2
    }
}