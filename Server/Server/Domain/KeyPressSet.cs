using System.Collections.Generic;

namespace Server.Server.Domain
{
    public class KeyPressSet : BaseEntity
    {
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; }

        public virtual List<KeyPress> KeyPresses { get; set; } = new List<KeyPress>();
    }
}