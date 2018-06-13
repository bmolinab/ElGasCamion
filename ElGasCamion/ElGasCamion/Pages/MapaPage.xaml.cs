using ElGasCamion.ViewModels;
using System;
using System.Collections.Generic;
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
	}
}