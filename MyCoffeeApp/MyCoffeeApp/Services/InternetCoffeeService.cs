using MonkeyCache.FileStore;
using MyCoffeeApp.Shared.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MyCoffeeApp.Services
{
    public static class InternetCoffeeService
    {
        static string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android ?
                                            "http://10.0.2.2:5000" : "http://localhost:5000";

        // static string BaseUrl = "YOUR URL";

        static HttpClient client;
        static HttpClientHandler handler;

        static InternetCoffeeService()
        {
            HttpClientHandler insecureHandler = GetInsecureHandler();
            client = new HttpClient(insecureHandler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public static async Task<IEnumerable<Coffee>> GetCoffee()
        {
            var json = await client.GetStringAsync("api/Coffee");
            var coffees = JsonConvert.DeserializeObject<IEnumerable<Coffee>>(json);
            return coffees;
        }

        static Random random = new Random();
        public static async Task AddCoffee(string name, string roaster)
        {
            var image = "https://www.yesplz.coffee/app/uploads/2020/11/emptybag-min.png";
            var coffee = new Coffee
            {
                Name = name,
                Roaster = roaster,
                Image = image,
                Id = random.Next(0, 10000)
            };

            var json = JsonConvert.SerializeObject(coffee);
            var content =
                new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Coffee", content);

            if(!response.IsSuccessStatusCode)
            {
                _ = Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Failed", "Failed to post coffee", "cancel");
            }
        }

        public static async Task RemoveCoffee(int id)
        {
            var response = await client.DeleteAsync($"api/Coffee/{id}");
            if (!response.IsSuccessStatusCode)
            {

            }
        }

        public static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }
    }
}
