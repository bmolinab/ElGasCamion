using ElGasCamion.Helpers;
using ElGasCamion.Models;
using ElGasCamion.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGasCamion.ViewModels
{
    public class RegisterViewModel
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Message { get; set; }

        public Distribuidor distribuidor { get; set; }

        #region Cosntructor
        public RegisterViewModel()
        {
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
                    var isRegistered = await _apiServices.RegisterUserAsync
                        (Username, Password, ConfirmPassword, distribuidor);

                    Settings.Username = Username;
                    Settings.Password = Password;

                    if (isRegistered)
                    {
                        Message = "Se registro con exito :)";
                    }
                    else
                    {
                        Message = "No  pudimos registrar su cuenta, reintentelo";
                    }
                });
            }
        }

        #endregion
    }
}
