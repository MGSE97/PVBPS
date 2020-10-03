using System.Collections.Generic;

namespace Server.Shared
{
    public class KeyPressSet
    {
        public int Id { get; set; }

        public virtual List<KeyPress> KeyPresses { get; set; } = new List<KeyPress>();
    }
}