﻿using ElGasCamion.Helpers;
using ElGasCamion.Models;
using ElGasCamion.Pages;
using ElGasCamion.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Messaging;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TK.CustomMap;
using TK.CustomMap.Api;
using TK.CustomMap.Api.Google;
using TK.CustomMap.Overlays;
using Xamarin.Forms;

namespace ElGasCamion.ViewModels
{
    public class MapaViewModel: INotifyPropertyChanged
    {
        public Compra compraresult = new Compra();
        public ObservableCollection<TKRoute> Routes { get; set; }
        ApiServices apiServices = new ApiServices();
        public MapSpan centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(0, 0)), Distance.FromMiles(.3)));
        public MapSpan CenterSearch
        {
            get { return centerSearch; }
            set
            {
                if (this.centerSearch != value)
                {

                    this.centerSearch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CenterSearch"));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<TKCustomMapPin> locations;
        public ObservableCollection<TKCustomMapPin> Locations
        {
            protected set
            {
                locations = Locations;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Locations"));
            }
            get { return locations; }
        }
        List<CompraResponse> ListaClientes = new List<CompraResponse>();
        public TKCustomMapPin myPin = new TKCustomMapPin();
        public TKCustomMapPin MyPin
        {
            get { return myPin; }
            set
            {
                if (this.myPin != value)
                {

                    this.myPin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MyPin"));
                }
            }
        }
        ObservableCollection<TKCircle> tkCircle = new ObservableCollection<TKCircle>();
        public ObservableCollection<TKCircle> TkCircle
        {
            get { return tkCircle; }
            set
            {
                if (this.tkCircle != value)
                {
                    this.tkCircle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TkCircle"));
                }
            }
        }

        public bool verMenu=false;

        public bool VerMenu
        {
            set
            {

                verMenu = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VerMenu"));

            }
            get
            {
                return verMenu;
            }
        }

        public bool verCompra = false;

        public bool VerCompra
        {
            set
            {

                verCompra = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VerCompra"));

            }
            get
            {
                return verCompra;
            }
        }

        private string tapItem = "";
        public string TapItem
        {
            get { return tapItem; }
            set { tapItem = value; }
        }

        bool tracking;
        public MapaViewModel()
        {

            if (!VerCompra)
            {
                centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(0, 0)), Distance.FromMiles(.3)));
            }
            Locations = new ObservableCollection<TKCustomMapPin>();
            locations = new ObservableCollection<TKCustomMapPin>();
            Routes = new ObservableCollection<TKRoute>();

            vendidos = "";
            //  Vendidos = "12";

            if (Settings.VenderGas)
            {

                dondeVender();
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                await loadParametros();
            });

                myposition();

           

            EntregasPendientes();
        }

        public void OnAppearing()
        {

            Locations = new ObservableCollection<TKCustomMapPin>();
            locations = new ObservableCollection<TKCustomMapPin>();


            centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(0, 0)), Distance.FromMiles(.3)));

            EntregasPendientes();
            //Do whatever you like in here

        }



        public void ActualizaFirebase()
        {
           
        }



        public async void dondeVender()
        {
            var compra = new Compra
            {
                IdCompra = Settings.IdCompra,
                
            };

            var response = await ApiServices.InsertarAsync<Compra>(compra, new Uri(Constants.BaseApiAddress), "/api/Compras/GetCompra");
             compraresult = JsonConvert.DeserializeObject<Compra>(response.Result.ToString());
            var color = new Color(0, 0, 255, 0.3);
            TK.CustomMap.Position centro = new TK.CustomMap.Position(latitude: compraresult.Latitud.Value,longitude:compraresult.Longitud.Value);
            var circle = new TKCircle { Radius = 700, Center = centro, Color = color};
            
            TkCircle.Add(circle);


            VerCompra = true;
            //  CenterSearch = new MapSpan(centro, centro.Latitude, centro.Longitude);

            CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(centro.Latitude, centro.Longitude)), Distance.FromMiles(5)));


        }

        public string vendidos { get; set; }
        public string Vendidos
        {
            set
            {
                if (vendidos != value)
                {
                    vendidos = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Vendidos"));
                }
            }
            get
            {
                return vendidos;
            }
        }


        public async Task<bool> loadParametros()
        {
            Cliente cliente = new Cliente
            {
                IdCliente = Settings.IdDistribuidor,
            };

            var response = await ApiServices.InsertarAsync<Cliente>(cliente, new Uri(Constants.BaseApiAddress), "/api/Parametroes/GetAllParameters");
            var parametros = JsonConvert.DeserializeObject<List<Parametro>>(response.Result.ToString());
            if (parametros != null)

            {
                bool Actualizado = true;
                foreach (var item in parametros)
                {
                  
                    if (item.Nombre == "versioncamion")
                    {
                        if (Constants.VersionCamion >= item.Valor)
                        {
                            Actualizado = true;
                        }
                        else
                        {
                            Actualizado = false;
                        }
                    }
                }
                if (!Actualizado) await App.Navigator.PushAsync(new UpdatePage());


            }
            return true;


        }



        public async void myposition()
        {
            try
            {
                var hasPermission = await Utils.CheckPermissions(Permission.Location);            
                if (!hasPermission)
                    return;
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 10;//DesiredAccuracy.Value;
                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(4), null, true);
                if (position == null)
                {
                    Debug.WriteLine("null gps :(");
                    return;
                }

                if (!VerCompra)
                {
                    CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(position.Latitude, position.Longitude)), Distance.FromMiles(1)));
                }

                //    CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                //    CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;

                //if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(20), 1,                       
                locator.PositionChanged += CrossGeolocator_Current_PositionChanged;
                if (!locator.IsListening)
                {
                    Debug.WriteLine("StartListeningAsync");
                    await locator.StartListeningAsync(TimeSpan.FromMinutes(5), 30);
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine("Uh oh, Something went wrong, but don't worry we captured for analysis! Thanks.");
            }
        }
        
        public async void EntregasPendientes()
        {
            Locations.Clear();
            Distribuidor distribuidor = new Distribuidor
            {
                IdDistribuidor=Settings.IdDistribuidor,
            };            
            var response = await ApiServices.InsertarAsync<Distribuidor>(distribuidor, new Uri(Constants.BaseApiAddress), "/api/Compras/MisVentasPendientes");
            ListaClientes = JsonConvert.DeserializeObject<List<CompraResponse>>(response.Result.ToString());
            Point p = new Point(0.48, 0.96);

            foreach (var cliente in ListaClientes)
            {
                var Pindistribuidor = new TKCustomMapPin
                {
                    Image = "casa",
                    Position = new TK.CustomMap.Position((double)cliente.Latitud, (double)cliente.Longitud),
                    Title = cliente.NombreCliente + "",
                    Subtitle ="Nro tanques: "+ cliente.Cantidad,
                    Anchor = p,                   
                    ShowCallout = true,
                };
                Locations.Add(Pindistribuidor);
            }
            ListaClientes.Count();            
        }

        public ICommand PinSelected { get { return new RelayCommand(pinselected); } }
        public async void pinselected()
        {
            VerMenu = true;
        }

        public ICommand ItemSelected { get { return new RelayCommand(itemselected); } }
        public async void itemselected()
        {

            switch (TapItem)
            {
                case "Llamar":
                    var cliente = ListaClientes.Where(x => x.NombreCliente == MyPin.Title).FirstOrDefault();

                    var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
                    if (PhoneCallTask.CanMakePhoneCall)
                    {
                        PhoneCallTask.MakePhoneCall(cliente.Telefono, cliente.NombreCliente);
                    }
                    break;
                case "Vender":
                   App.clienteseleccionado = ListaClientes.Where(x => x.NombreCliente == MyPin.Title).FirstOrDefault();
                    await App.Navigator.PushAsync(new DetallePage());
                    break;
                case "Ruta":
                    Routes.Clear();
                    TKRoute route = new TKRoute
                    {
                        TravelMode = TKRouteTravelMode.Driving,
                        Source = CenterSearch.Center,
                        Destination = MyPin.Position,                        
                        Color = Color.Blue,    
                        LineWidth=5
                    };

                    Routes.Add(route);
                    Debug.WriteLine(route.Distance);
                    Routes.Count();
                    break;
            }
        }

        public ICommand PinUnselected { get { return new RelayCommand(pinunselected); } }
        public async void pinunselected()
        {
            try
            {
                VerMenu = false;
                MyPin = null;
               // App.clienteseleccionado = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ICommand AplicarCommand { get { return new RelayCommand(aplicarCommand); } }
        public async void aplicarCommand()
        {
            try
            {
                compraresult.IdDistribuidor = Settings.IdDistribuidor;
                var response = await ApiServices.InsertarAsync<Compra>(compraresult, new Uri(Constants.BaseApiAddress), "/api/Compras/Aplicar");
            //    var compraresult = JsonConvert.DeserializeObject<Compra>(response.Result.ToString());
                if(response.IsSuccess)
                {
                    Settings.VenderGas = false;
                    Settings.IdCompra = 0;
                    TkCircle.Clear();
                    VerCompra = false;
                    EntregasPendientes();
                }
                else 
                {
                    Settings.VenderGas = false;
                    Settings.IdCompra = 0;
                    TkCircle.Clear();
                    VerCompra = false;

                }

                // App.clienteseleccionado = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ICommand NoAplicarCommand { get { return new RelayCommand(noaplicarCommand); } }


        public async void noaplicarCommand()
        {
            try
            {
                Settings.VenderGas = false;
                Settings.IdCompra = 0;
                TkCircle.Clear();
                VerCompra = false;
                // App.clienteseleccionado = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e)
        {
            Debug.WriteLine("Location error: " + e.Error.ToString());
        }

        void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e)
        {
           
              

            Device.BeginInvokeOnMainThread(async () =>
            {
                await apiServices.UpdatePosition(new Ruta
                {
                    IdDistribuidor = Settings.IdDistribuidor,
                    Latitud = e.Position.Latitude,
                    Longitud = e.Position.Longitude,

                });

                await apiServices.LogRuta(new Ruta
                {
                    IdDistribuidor = Settings.IdDistribuidor,
                    Latitud = e.Position.Latitude,
                    Longitud = e.Position.Longitude,

                });


            });





        }
    }
}
