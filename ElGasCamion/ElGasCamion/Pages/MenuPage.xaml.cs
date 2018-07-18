using ElGasCamion.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGasCamion.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MenuViewModel viewModel;
        public MenuPage()
        {
            viewModel = new MenuViewModel();
            BindingContext = viewModel;

            InitializeComponent();
        }
    }
}