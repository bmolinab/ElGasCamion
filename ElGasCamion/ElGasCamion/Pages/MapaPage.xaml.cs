using ElGasCamion.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGasCamion.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapaPage : ContentPage
	{
        MapaViewModel viewModel = new MapaViewModel();
        public MapaPage()
        {
            InitializeComponent();
            
            BindingContext = viewModel;          
        }

        protected override void OnAppearing()
        {
           // base.OnAppearing();
            viewModel = new MapaViewModel();
            BindingContext = viewModel;

            //your code here;

        }

        private void TKCustomMap_RouteCalculationFinished(object sender, TK.CustomMap.TKGenericEventArgs<TK.CustomMap.Overlays.TKRoute> e)
        {
            Debug.WriteLine("se cargo la ruta");
        }

        private void TKCustomMap_RouteCalculationFailed(object sender, TK.CustomMap.TKGenericEventArgs<TK.CustomMap.Models.TKRouteCalculationError> e)
        {
            
            Debug.WriteLine(e.Value.ErrorMessage);

        }
    }
}