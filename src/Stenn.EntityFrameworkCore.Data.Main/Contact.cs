namespace Stenn.EntityFrameworkCore.Data.Main
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public ContactType Type { get; set; }
    }
}