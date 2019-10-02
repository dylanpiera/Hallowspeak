using System.Collections.Generic;

namespace Hallowspeak.Data.Models
{
    /// <summary>
    /// The data for a single row in the Lexicon
    /// </summary>
    public class LexiconItem
    {
        /// <summary>
        /// Word UID, also used for the database.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// To generalize the Database Structure, and not have to worry about changing said structure,
        /// the data pulled from the database is stored in a Dictionary, where the Key is the name of the DB Column
        /// and the value is the corresponding value from the DB.
        /// </summary>
        public Dictionary<string, LexiconValue> KeyValues = new Dictionary<string, LexiconValue>();

        public LexiconItem(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Used for two-way data-binding in for-each loops. See Shared/Lexicon/LexiconRow.razor
    /// </summary>
    public class LexiconValue
    {
        public string Value { get; set; }

        public LexiconValue(string value)
        {
            Value = value;
        }
    }
}
