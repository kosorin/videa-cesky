using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class Tag : BindableBase
    {
        private string _feed = "";
        public string Feed
        {
            get { return _feed; }
            set { SetProperty(ref _feed, value); }
        }

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public Tag(string feed, string name)
        {
            Feed = feed;
            Name = name;
        }
    }

    public class Video : BindableBase
    {
        #region Uri
        private Uri _uri;
        public Uri Uri
        {
            get { return _uri; }
            set { SetProperty(ref _uri, value); }
        }
        #endregion

        #region Title
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion

        #region Detail
        private string _detail;
        public string Detail
        {
            get { return _detail; }
            set { SetProperty(ref _detail, value); }
        }
        #endregion

        #region ImageUri
        private Uri _imageUri;
        public Uri ImageUri
        {
            get { return _imageUri; }
            set { SetProperty(ref _imageUri, value); }
        }
        #endregion

        #region Date
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
        #endregion

        #region Tags
        private List<Tag> _tags = null;
        public List<Tag> Tags
        {
            get { return _tags; }
            set { SetProperty(ref _tags, value); }
        }
        #endregion // end of Tags

        #region Rating
        private double _rating = 0;
        public double Rating
        {
            get { return _rating; }
            set { SetProperty(ref _rating, value); }
        }
        #endregion // end of Rating
    }
}
