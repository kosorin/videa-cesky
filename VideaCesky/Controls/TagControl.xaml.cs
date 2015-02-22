using MyToolkit.Paging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideaCesky.Models;
using VideaCesky.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky.Controls
{
    public sealed partial class TagControl : UserControl
    {
        public TagControl()
        {
            this.InitializeComponent();
        }

        private void TagButton_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
            {
                Button button = sender as Button;
                Tag tag = button.DataContext as Tag;

                MenuFlyout mf = new MenuFlyout();
                MenuFlyoutItem item = new MenuFlyoutItem();
                if (Settings.Current.SavedTags.Contains(tag))
                {
                    item.Text = "smazat";
                    item.Click += async (ss, ee) => { await Settings.Current.RemoveTag(tag); };
                }
                else
                {
                    item.Text = "uložit";
                    item.Click += async (ss, ee) => { await Settings.Current.AddTagAsync(tag); };
                }
                mf.Items.Add(item);
                mf.ShowAt(button);
            }
            e.Handled = true;
        }

        private async void TagButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                MtFrame frame = Window.Current.Content as MtFrame;
                if (frame != null)
                {
                    await frame.NavigateAsync(typeof(CategoryPage), fe.DataContext as Tag);
                }
            }
        }
    }
}
