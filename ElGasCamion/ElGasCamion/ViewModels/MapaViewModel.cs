using ElGasCamion.Helpers;
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



        //public ObservableCollection<TKRoute> routes;
        //public ObservableCollection<TKRoute> Routes
        //{
        //    protected set
        //    {
        //        routes = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Routes"));
        //    }
        //    get { return routes; }
        //}

        public ObservableCollection<TKRoute> Routes { get; set; }

        public MapSpan centerSearch = null;
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
        private string tapItem = "";


        public string TapItem
        {
            get { return tapItem; }
            set { tapItem = value; }

        }


        bool tracking;
        public MapaViewModel()
        {
            Locations = new ObservableCollection<TKCustomMapPin>();
            locations = new ObservableCollection<TKCustomMapPin>();
            Routes = new ObservableCollection<TKRoute>();
          
            vendidos = "";
          //  Vendidos = "12";

            myposition();
            EntregasPendientes();
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
                CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(position.Latitude, position.Longitude)), Distance.FromMiles(1)));



                if (tracking)
                {
                    CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                    CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;
                }
                else
                {
                    CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                    CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
                }
              


                if (CrossGeolocator.Current.IsListening)
                {
                    await CrossGeolocator.Current.StopListeningAsync();
                    //labelGPSTrack.Text = "Stopped tracking";
                    //ButtonTrack.Text = "Start Tracking";
                    tracking = false;
                    //count = 0;
                }
                else
                {
                    // Positions.Clear();
                    if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(30), 1,
                        false, new ListenerSettings
                        {

                        }))
                    {
                        //labelGPSTrack.Text = "Started tracking";
                        //ButtonTrack.Text = "Stop Tracking";
                        tracking = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Uh oh, Something went wrong, but don't worry we captured for analysis! Thanks.");
            }
        }

        
        public async void EntregasPendientes()
        {
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
                    Image = "cliente01",
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


        void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e)
        {

            Debug.WriteLine("Location error: " + e.Error.ToString());
        }

        void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e)
        {

            Device.BeginInvokeOnMainThread(() =>
            {

            });
        }
    }
}
