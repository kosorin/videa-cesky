using MyToolkit.Paging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VideaCesky.Pages;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideaCesky
{
    public class MyApp : MtApplication
    {
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
    }

    public sealed partial class App : MyApp
    {
        public App()
        {
            this.InitializeComponent();
        }
    }
}