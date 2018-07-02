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
	public partial class DetallePage : ContentPage
	{
        DetalleViewModel viewModel = new DetalleViewModel();

        public DetallePage ()
		{
			InitializeComponent ();
            BindingContext = viewModel;
		}
	}
}