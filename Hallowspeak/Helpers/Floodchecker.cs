using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Hallowspeak.Helpers
{
    public class Floodchecker
    {
        private HashSet<IPAddress> _blacklist = new HashSet<IPAddress>();

        public void Timeout(IPAddress ip, TimeSpan duration)
        {
            _blacklist.Add(ip);
            Task.Run(async () => { 
                await Task.Delay(duration); 
                _blacklist.Remove(ip); 
            });
        }

        public void Timeout(IPAddress ip) => Timeout(ip, new TimeSpan(0, 1, 0));

        public bool IsTimedOut(IPAddress ip) => _blacklist.Contains(ip);
    }
}
