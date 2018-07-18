using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElGasCamion.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault("Username", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Username", value);
            }
        }

        public static int IdCompra
        {
            get
            {
                return AppSettings.GetValueOrDefault("IdCompra", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("IdCompra", value);
            }
        }

        public static bool VenderGas
        {
            get
            {
                return AppSettings.GetValueOrDefault("VenderGas", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("VenderGas", value);
            }
        }

        public static string DeviceID
        {
            get
            {
                return AppSettings.GetValueOrDefault("DeviceID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("DeviceID", value);
            }
        }

        public static string Password
        {
            get
            {
                return AppSettings.GetValueOrDefault("Password", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Password", value);
            }
        }
        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessToken", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessToken", value);
            }
        }

        public static DateTime AccessTokenExpirationDate
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessTokenExpirationDate", DateTime.UtcNow);
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessTokenExpirationDate", value);
            }
        }

        public static int IdDistribuidor
        {
            get
            {
                return AppSettings.GetValueOrDefault("IdDistribuidor", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("IdDistribuidor", value);
            }
        }


    }

}
