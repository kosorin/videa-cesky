﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class Category : BindableBase
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _feed = "";
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
