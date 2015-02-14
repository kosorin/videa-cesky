using MyToolkit.Messaging;
using MyToolkit.Paging;
using MyToolkit.Paging.Animations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideaCesky.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky
{
    public sealed partial class App : MtApplication
    {
        public App()
            : base()
        {
            this.InitializeComponent();
        }

        public override Type StartPageType
        {
            get { return typeof(MainPage); }
        }

        public override Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            return null;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await InitializeFrameAsync(args.PreviousExecutionState);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                Uri uri = ((ProtocolActivatedEventArgs)args).Uri;
                EnsureCreatedAndActivated(uri.AbsolutePath);
            }
            else
            {
                EnsureCreatedAndActivated();
            }
        }

        private void EnsureCreatedAndActivated(string uri = null)
        {
            Frame rootFrame;
            if (uri != null)
            {
                // Máme nové video, proto vytvoříme vždy nový Frame.
                rootFrame = null;
            }
            else
            {
                rootFrame = Window.Current.Content as Frame;
            }

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.CacheSize = 3;
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(uri != null ? typeof(VideoPage) : typeof(MainPage), uri))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            Window.Current.Activate();
        }
    }
}