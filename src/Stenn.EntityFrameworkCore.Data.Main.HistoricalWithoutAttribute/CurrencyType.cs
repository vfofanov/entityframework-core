using System.ComponentModel.DataAnnotations;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute
{
    [EnumTable("CurrencyTypes")]
    public enum CurrencyType
    {
        [Display(Name = "National currency")] 
        National = 1,

        [Display(Name = "Cripto currency")] 
        Cripto = 2
    }
}