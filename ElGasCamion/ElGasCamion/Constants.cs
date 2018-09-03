using ElGasCamion.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ElGasCamion
{
    public class Constants
    {
        public static string BaseApiAddress => "http://52.224.8.198:58/";
        #region DataGCM
        public const string SenderID = "5570742533"; // Google API Project Number
        public const string ListenConnectionString = "Endpoint=sb://notificacionesdesarrollo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B10SdmICrxrk4RAZ3abyGBB3SdrntdmW+iKImvByLIQ=";
        public const string NotificationHubName = "notificacionesds";

        public const string USER_AGENT = "firebase-net/1.0";
        public const string FirebaseURI = "https://elgas-f24e8.firebaseio.com/-LJVkHULelfySFjNF9-Q/Equipo-ElGas/Distribuidores/";

        public static double VersionCamion = 1;


        #endregion


    }
}
