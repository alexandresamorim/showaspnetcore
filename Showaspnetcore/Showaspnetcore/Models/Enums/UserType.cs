using System.ComponentModel.DataAnnotations;

namespace Showaspnetcore.Models.Enums
{
    public enum UserType
    {
        [Display(Name = "Courier")]
        COURIER,

        [Display(Name = "Dispatcher")]
        DISPATCHER,

        [Display(Name = "Accountant")]
        ACCOUNTANT,

        [Display(Name = "Normal User")]
        NORMAL_USER
    }
}