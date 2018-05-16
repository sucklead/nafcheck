using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nafcheck
{
    class FileAccess
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public byte[] ByteContent { get; set; }
        public int PotentialLengthValue { get; set; }
        public bool LengthFromPrevious { get; set; }

        public FileAccess(int offset, int length)
        {
            Offset = offset;
            Length = length;
            ByteContent = new byte[length];
            PotentialLengthValue = 0;
            LengthFromPrevious = false;
        }
    }
}
