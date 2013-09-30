using System.Diagnostics;
using System.IO;
using System.Text;

namespace e9.Debugging
{
    public class DebugTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(char value)
        {
            Debug.Write(value);
        }

        public override void Write(string value)
        {
            Debug.Write(value);
        }

        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }
    }
}