using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TK.CustomMap.Droid;
using Plugin.Permissions;
using Android.Util;

namespace ElGasCamion.Droid
{
    [Activity(Label = "ElGasCamion", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string TAG = "MainActivity";

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
             TKGoogleMaps.Init(this, bundle);
          

            base.OnCreate(bundle);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key != null)
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    }
                }
            }


            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

