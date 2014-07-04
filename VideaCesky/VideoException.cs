using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class VideoException : Exception
    {
        public VideoException(string message = "Neznámá chyba.")
            : base(message) { }
    }
}
