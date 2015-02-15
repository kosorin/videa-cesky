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
    public class Category : BindableBase, ICategory
    {
        private string _name = "";
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description = "";
        [DataMember]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _feed = "";
        [DataMember]
        public string Feed
        {
            get { return _feed; }
            set { SetProperty(ref _feed, value); }
        }

        public Category(string name, string description, string feed)
        {
            Name = name;
            Description = description;
            Feed = feed;
        }
    }
}
