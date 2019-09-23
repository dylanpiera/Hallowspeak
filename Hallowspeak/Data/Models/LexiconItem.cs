using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallowspeak.Data.Models
{
    public class LexiconItem
    {
        public int Id { get; private set; }
        public Dictionary<string, LexiconValue> KeyValues = new Dictionary<string, LexiconValue>();

        public LexiconItem(int id)
        {
            Id = id;
        }
    }

    public class LexiconValue
    {
        public string Value { get; set; }

        public LexiconValue(string value)
        {
            Value = value;
        }
    }
}
