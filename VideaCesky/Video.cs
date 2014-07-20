using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky
{
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
    }
}
