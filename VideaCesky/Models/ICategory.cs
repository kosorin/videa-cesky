using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky.Models
{
    public interface ICategory
    {
        string Feed { get; set; }

        string Name { get; set; }
    }
}
