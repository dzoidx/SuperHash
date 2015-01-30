using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperHash
{
    public class Chunk
    {
        public long FromOffset;
        public long ToOffset;
        public byte[] Hash;

        public string ToString(int chunkNo)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Chunk #{0,4}", chunkNo);
            sb.AppendFormat(" {0,10}...{1,10}", FromOffset, ToOffset);
            sb.AppendFormat(": {0}", BitConverter.ToString(Hash).Replace("-", ""));

            return sb.ToString();
        }
    }
}
