using System;
using Server.Shared;

namespace Server.Server.Domain
{
    public class KeyPress : BaseEntity
    {
        public int SetId { get; set; }
        public virtual KeyPressSet Set { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int Code { get; set; }

        public KeyPressDataType? DataType { get; set; }
        public string Data { get; set; }
    }
}