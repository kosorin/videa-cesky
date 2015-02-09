using VideaCesky.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VideaCesky
{
    public sealed partial class CategoryPage : VideoListBasePage
    {
        private Category _category = null;
        public Category Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public CategoryPage()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        protected override VideoList GetVideListControl()
        {
            return VideoListControl;
        }

        protected override void SetFeed(object parameter)
        {
            if (parameter is Category)
            {
                Category = parameter as Category;
                VideoList.Feed = Category.Feed;
            }
            else
            {
                base.SetFeed(parameter);
            }
        }
    }
}
