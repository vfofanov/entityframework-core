using System.ComponentModel.DataAnnotations;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    public enum ContactStr2Type : byte
    {
        [Display(Name = "Person str contact 2")] 
        Person2 = 1,

        [Display(Name = "Organization str contact 2")]
        Organization2 = 2
    }
}