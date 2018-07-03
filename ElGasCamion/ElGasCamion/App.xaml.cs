using ElGasCamion.Helpers;
using ElGasCamion.Models;
using ElGasCamion.Pages;
using ElGasCamion.Services;
using ElGasCamion.ViewModels;
using System;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ElGasCamion
{
	public partial class App : Application
	{
        public static MasterPage Master { get; internal set; }
        public static NavigationPage Navigator { get; internal set; }
        public static CompraResponse clienteseleccionado { get; internal set; }

        public App ()
		{
			InitializeComponent();

            GmsPlace.Init("AIzaSyDAmhu79jCKlkE6KIVSqgxlIl83gJj_rkk");
            GmsDirection.Init("AIzaSyDAmhu79jCKlkE6KIVSqgxlIl83gJj_rkk");

            NavigationService navigationService = new NavigationService();
            navigationService.SetMainPage();

            //MainPage = new MainPage();
        }
        private void SetMainPage()
        {
            if (!string.IsNullOrEmpty(Settings.AccessToken))
            {
                if (Settings.AccessTokenExpirationDate < DateTime.UtcNow.AddHours(1))
                {
                    var loginViewModel = new LoginViewModel();
                    loginViewModel.LoginCommand.Execute(null);
                }
                MainPage = new NavigationPage(new MapaPage());
            }
            else if (!string.IsNullOrEmpty(Settings.Username)
                  && !string.IsNullOrEmpty(Settings.Password))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }


        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
