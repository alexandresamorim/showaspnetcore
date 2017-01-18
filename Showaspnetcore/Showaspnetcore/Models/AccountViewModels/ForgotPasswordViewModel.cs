using System.ComponentModel.DataAnnotations;

namespace Showaspnetcore.Model.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}