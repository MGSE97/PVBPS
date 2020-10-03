using System.Collections.Generic;

namespace Server.Server.Models
{
    public class LogRequest
    {
        public string Key { get; set; }
        public IList<Shared.KeyPress> Keys { get; set; } = new List<Shared.KeyPress>();
    }
}