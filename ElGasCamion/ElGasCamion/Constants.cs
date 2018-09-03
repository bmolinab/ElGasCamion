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
        #endregion

       
    }
}
