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


        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        public async void Cancel()
        {

            var action = await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", string.Format("{0} Tanque(s)", Detalle.Cantidad), "Confirmar", "Regresar");
            if (action)
            {
                var compra = new Compra { IdCompra = Settings.IdCompra, IdDistribuidor = Settings.IdDistribuidor };
                var compracancelada = new CompraCancelada
                {
                    IdCompra = Detalle.IdCompra,
                    IdDistribuidor = Settings.IdDistribuidor,
                    CanceladaPor = 2,
                    IdCliente = (int)Detalle.IdCliente
                };
                var response = await ApiServices.InsertarAsync<CompraCancelada>(compracancelada, new Uri(Constants.BaseApiAddress), "/api/Compras/Cancelar");
                if (response.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", string.Format("Su pedido de {0} tanque(s) ha sido cancelado", Detalle.Cantidad), "Aceptar");
                    await App.Navigator.Navigation.PopToRootAsync();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Problemas", string.Format("Tenemos problemas para cancelarsu pedido de {0} tanque(s), trabajamos para solucionarlo", Detalle.Cantidad), "Aceptar");
                    //Settear las variables globales              
                    await App.Navigator.Navigation.PopToRootAsync();
                }



            }

        }


    }
}
