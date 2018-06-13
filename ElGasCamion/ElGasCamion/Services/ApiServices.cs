using ElGasCamion.Helpers;
using ElGasCamion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ElGasCamion.Services
{
    internal class ApiServices
    {
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword, Distribuidor distribuidor)
        {
            var client = new HttpClient();

            var model = new RegisterBindingModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var json = JsonConvert.SerializeObject(model);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(
                Constants.BaseApiAddress + "api/Account/Register", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            var AspNetUSer = JsonConvert.DeserializeObject<AspNetUser>(result);

            distribuidor.IdAspNetUser = AspNetUSer.Id;
            distribuidor.Correo = AspNetUSer.Email;



            Debug.WriteLine(result);
            if (response.IsSuccessStatusCode)
            {
                var json2 = JsonConvert.SerializeObject(distribuidor);
                HttpContent httpContent2 = new StringContent(json2);

                httpContent2.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                var response2 = await client.PostAsync(
                Constants.BaseApiAddress + "api/Distribuidors/PostDistribuidor", httpContent2);

                if (response2.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post, Constants.BaseApiAddress + "Token");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(content);

            var accessTokenExpiration = jwtDynamic.Value<DateTime>(".expires");
            var accessToken = jwtDynamic.Value<string>("access_token");

            Settings.AccessTokenExpirationDate = accessTokenExpiration;

            Debug.WriteLine(accessTokenExpiration);

            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                var model = new RegisterBindingModel
                {
                    Email = userName,
                    Password = password,
                    ConfirmPassword = password
                };

                var json2 = JsonConvert.SerializeObject(model);
                HttpContent httpContent2 = new StringContent(json2);

                httpContent2.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                var response2 = await client.PostAsync(
                Constants.BaseApiAddress + "api/Distribuidors/GetforUser", httpContent2);

                if (response2.IsSuccessStatusCode)
                {
                    var result = await response2.Content.ReadAsStringAsync();

                    var distribuidor = JsonConvert.DeserializeObject<Distribuidor>(result);
                    Settings.IdDistribuidor = distribuidor.IdDistribuidor;
                }
            }






            return accessToken;

            //antes de retornar el token debemos obtener el idDistribuidor
        }

        public async Task<bool> LogRuta(Ruta ruta)
        {
            try
            {
                var request = JsonConvert.SerializeObject(ruta);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(Constants.BaseApiAddress);
                var url = "api/Rutas/PostRutas";
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                   
                }
                return true;
                //  var log = JsonConvert.DeserializeObject<LogPosition>(result);            
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;

            }
        }

    }
}
