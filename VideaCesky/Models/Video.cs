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
    [DataContract]
    public class Video
    {
        [DataMember]
        public Uri Uri { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Detail { get; set; }

        [DataMember]
        public Uri ImageUri { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public List<Tag> Tags { get; set; }

        [DataMember]
        public double Rating { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Video);
        }

        public bool Equals(Video obj)
        {
            return obj != null && obj.Uri == this.Uri;
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }
    }
}
