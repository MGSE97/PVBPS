using System.Collections.Generic;

namespace Server.Server.Domain
{
    public class Machine : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Key { get; set; }

        public List<KeyPressSet> KeyPressSets { get; set; } = new List<KeyPressSet>();
    }
}