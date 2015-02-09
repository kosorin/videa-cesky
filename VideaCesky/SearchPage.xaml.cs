using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace VideaCesky
{
    public sealed partial class SearchPage : VideoListBasePage
    {
        public SearchPage()
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
            base.SetFeed(parameter);
            VideoList.Search = SearchExpression.Text = (parameter as string) ?? "";
        }
    }
}
