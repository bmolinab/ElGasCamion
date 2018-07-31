using ElGasCamion.Helpers;
using ElGasCamion.Models;
using ElGasCamion.Pages;
using ElGasCamion.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGasCamion.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly ApiServices _apiServices = new ApiServices();

        #region Properties

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string identificacion { get; set; }
        public string message = "";
        public string Message
        {
            set
            {
                if (message != value)
                {
                    message = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Message"));
                }
            }
            get
            {
                return message;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isError=false;
        public bool IsError
        {
            set
            {
                if (isError != value)
                {
                    isError = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsError"));
                }
            }
            get
            {
                return isError;
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsBusy"));
                }
            }
            get
            {
                return isBusy;
            }
        }
        #endregion

        //


        public Distribuidor distribuidor { get; set; }

        #region Cosntructor
        public RegisterViewModel()
        {
            Message = "";
            isError = false;
            IsError = false;
               distribuidor = new Distribuidor();
        }
        #endregion

        #region Commands
        public ICommand RegisterCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsBusy = true;


                if (Password != null && Password != "")
                {
                        if (ConfirmPassword == Password)
                        {
                            if (Password.Length > 3)
                            {
                                distribuidor.Habilitado = false;
                                var isRegistered = await _apiServices.RegisterUserAsync
                                
                               (Username, Password, ConfirmPassword, distribuidor);

                                Settings.Username = Username;
                                Settings.Password = Password;

                                if (isRegistered)
                                {
                                    IsBusy = false;

                                    Message = "Se registró con éxito";
                                    await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                                    App.Current.MainPage = new NavigationPage(new LoginPage());
                                }
                                else
                                {
                                    IsBusy = false;
                                    Message = "Error al registrar su cuenta, reintentelo";
                                    await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                                }
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Las contraseña debe tener al menos 4 caracteres", "Aceptar");

                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "Aceptar");
                        }

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios", "Aceptar");

                    }





                });
            }
        }

        #endregion
    }
}
