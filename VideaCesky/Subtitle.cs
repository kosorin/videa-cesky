using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky
{
    public struct Subtitle
    {
        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public string Text { get; set; }
    }
}
