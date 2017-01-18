using System.ComponentModel.DataAnnotations;

namespace Showaspnetcore.Model.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}