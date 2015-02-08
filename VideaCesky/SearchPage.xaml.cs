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
            : base()
        {
            this.InitializeComponent();
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

        protected override void OnStartRefreshing()
        {
            NoResultTextBlock.Visibility = Visibility.Collapsed;
        }

        protected override void OnRefresh()
        {
            if (VideoList.List.Count == 0)
            {
                NoResultTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
