using ElGasCamion.Helpers;
using ElGasCamion.Models;
using ElGasCamion.Pages;
using ElGasCamion.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using TK.CustomMap;
using TK.CustomMap.Api;
using TK.CustomMap.Overlays;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ElGasCamion.ViewModels
{
    public class DetalleViewModel : INotifyPropertyChanged
    {
        

        public event PropertyChangedEventHandler PropertyChanged;
        private CompraResponse detalle = new  CompraResponse();
        public CompraResponse Detalle
        {
            get { return detalle; }
            set { detalle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Detalle"));}

        }

        public string direccion = "";
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Direccion")); }

        }

        Geocoder geoCoder;

        public DetalleViewModel()
        {
            geoCoder = new Geocoder();

            Detalle = App.clienteseleccionado;

            ObtenerDireccion();
        }

        async void  ObtenerDireccion()
        {
            var position = new Xamarin.Forms.Maps.Position(Detalle.Latitud.Value, Detalle.Longitud.Value);
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
                       
            foreach (var address in possibleAddresses)
            {
                Direccion = address;
                break;
            }
        }

        public ICommand OkCommand { get { return new RelayCommand(Ok); } }
        private async void Ok()
        {
            ApiServices apiServices = new ApiServices();
            var response = await ApiServices.InsertarAsync<CompraResponse>(Detalle, new Uri(Constants.BaseApiAddress), "/api/Compras/Vender");
            if (response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Entrega", "Pedido Entregado", "Aceptar");                
                await App.Navigator.PopToRootAsync();

            }
            else
            {
                    await App.Current.MainPage.DisplayAlert("Tenemos un problema con su pedido", response.Message, "Aceptar");
            }           
        }
    }
}
