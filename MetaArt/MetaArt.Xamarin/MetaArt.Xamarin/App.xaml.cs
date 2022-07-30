using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MetaArt.Xamarin
{
    public partial class App : Application
    {
        public App ()
        {
            Routing.RegisterRoute("sketch", typeof(SkecthPage));
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

