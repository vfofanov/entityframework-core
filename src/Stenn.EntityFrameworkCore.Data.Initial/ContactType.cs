using System.ComponentModel.DataAnnotations;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.Data.Initial
{
    [EnumTable("CurrencyTypes")]
    public enum CurrencyType
    {
        [Display(Name = "National currency")] 
        National = 0,

        [Display(Name = "Cripto currency")] 
        Cripto = 1
    }
}