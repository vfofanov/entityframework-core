using System.Linq;
using System.Text;

namespace Stenn.EntityDefinition
{
    public sealed class CsvBuilder
    {
        private char Delimiter { get; }

        private readonly char[] _escapingChars;

        private readonly StringBuilder _sbuilder = new();

        public CsvBuilder(char delimiter = ',')
        {
            Delimiter = delimiter;
            _escapingChars = new[] { Delimiter, '"', '\r', '\n', '\t', ' ' };
        }

        private string GetCsvValue(string? value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (_escapingChars.Intersect(value).Any())
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        public void AddRow(string?[] values)
        {
            _sbuilder.AppendLine(string.Join(Delimiter, values.Select(GetCsvValue)));
        }

        public string Build()
        {
            return _sbuilder.ToString();
        }
    }
}