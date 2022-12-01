﻿namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public ContactType Type { get; set; }
        public ContactStrType TypeName { get; set; }
        
        public ContactStr2Type TypeName2 { get; set; }
        
        public ContactTypeNullable? TypeNameNullable { get; set; }
    }
}