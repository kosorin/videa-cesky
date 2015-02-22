using MyToolkit.Paging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VideaCesky.Models;
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

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Debug.WriteLine("____ ON LAUNCHED");
            await Settings.LoadAsync();
            await InitializeFrameAsync(args.PreviousExecutionState);
        }

        public override Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            Debug.WriteLine("____ INITIALIZED");
            return null;
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