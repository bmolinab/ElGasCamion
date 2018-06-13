﻿using ElGasCamion.Helpers;
using ElGasCamion.Pages;
using ElGasCamion.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGasCamion.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #region Services
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ApiServices _apiServices = new ApiServices();
        #endregion

        #region Propieties
        private bool _isRemember = false;
        public bool isRemember
        {
            set
            {
                if (_isRemember != value)
                {
                    _isRemember = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("isRemember"));
                }
            }
            get
            {
                return _isRemember;
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }

        
        #endregion
        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var accesstoken = await _apiServices.LoginAsync(Username, Password);

                    if (accesstoken != null)
                    {
                        Settings.AccessToken = accesstoken;

                        
                        var idDistribuidor = await _apiServices.LoginAsync(Username, Password);

                        App.Current.MainPage = new NavigationPage(new MapaPage());
                    }

                });
            }
        }


        public ICommand RegisterCommand { get { return new RelayCommand(Register); } }

        private async void Register()
        {
            App.Current.MainPage = new NavigationPage(new RegisterPage());
        }

        #endregion


        #region Constructor
        public LoginViewModel()
        {
            Username = Settings.Username;
            Password = Settings.Password;
        }
        #endregion
    }
}
