using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VideaCesky.Common;

namespace VideaCesky.Models
{
    public class Video : BindableBase
    {
        public Uri Uri { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public Uri ImageUri { get; set; }

        public DateTime Date { get; set; }

        public List<Tag> Tags { get; set; }

        public double Rating { get; set; }
    }
}
