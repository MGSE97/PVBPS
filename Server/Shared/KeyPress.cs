using System;

namespace Server.Shared
{
    public class KeyPress
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int Code { get; set; }
        public Keys Key => (Keys) Code;

        public KeyPressDataType? DataType { get; set; }
        public string Data { get; set; }
    }

    [Flags]
    public enum KeyPressDataType
    {
        Copy = 1,
        Paste = 2,

        Clipboard = 16,

        Text = 128,
        Image = 256,
        Audio = 512,
        Data = 1024
    }
}