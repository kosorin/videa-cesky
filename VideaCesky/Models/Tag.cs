using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VideaCesky.Common;

namespace VideaCesky.Models
{
    [DataContract]
    public class Tag : BindableBase, ICategory
    {
        [DataMember]
        public string Feed { get; set; }

        [DataMember]
        public string Name { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public Tag(string feed, string name)
        {
            Feed = feed;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Tag);
        }

        public bool Equals(Tag obj)
        {
            return obj != null && obj.Feed == this.Feed;
        }

        public override int GetHashCode()
        {
            return Feed.GetHashCode();
        }
    }
}
