using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallowspeak.Data.Models
{
    public struct DiscordUser
    {
        public string UserID { get; set; }
        public string Username { get; set; }

        public AuthLevel AuthLevel { get; set; }
    }

    public enum AuthLevel
    {
        User = 0,
        Writer = 1,
        Administrator = 2
    }
}
