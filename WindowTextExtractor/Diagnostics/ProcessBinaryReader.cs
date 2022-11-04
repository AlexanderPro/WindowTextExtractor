//******************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/ProcessBinaryReader.cs
//******************************************************************************************************************************

using System.IO;
using System.Text;

namespace WindowTextExtractor.Diagnostics
{
    sealed class ProcessBinaryReader : BinaryReader
    {
        public ProcessBinaryReader(Stream input, Encoding encoding) :
            base(input, encoding)
        {
        }

        public string ReadCString()
        {
            var sb = new StringBuilder();

            for (; ; )
            {
                int c = Read();
                if (c == -1)
                    throw new EndOfStreamException();

                if (c == 0)
                {
                    // End of string.
                    break;
                }

                sb.Append((char)c);
            }

            return sb.ToString();
        }
    }
}
