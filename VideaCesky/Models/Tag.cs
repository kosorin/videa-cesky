using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideaCesky.Common;

namespace VideaCesky.Models
{
    public class Tag : BindableBase, ICategory
    {
        public string Feed { get; set; }

        public string Name { get; set; }

        public Tag(string feed, string name)
        {
            Feed = feed;
            Name = name;
        }
    }
}
