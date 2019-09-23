using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallowspeak.Data.Models
{
    public class LexiconItem
    {
        public int Id { get; private set; }
        public Dictionary<string, string> KeyValues = new Dictionary<string, string>();

        public LexiconItem(int id)
        {
            Id = id;
        }
    }
}
