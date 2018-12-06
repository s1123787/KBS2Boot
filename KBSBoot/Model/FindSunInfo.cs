using KBSBoot.Resources;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    class FindSunInfo
    {
        public static string GetUnFormattedSunInfo(double lat, double lng, DateTime date)
        {
            var dt = date.ToString("yyyy-MM-dd");

            var client = new RestClient($"https://api.sunrise-sunset.org/json?lat={lat}&lng={lng}&date={dt}");

            var response = client.Execute(new RestRequest());

            return response.Content;
        }

        public static ObjectResults GetSunInfo(double lat, double lng, DateTime date)
        {
            var dt = date.ToString("yyyy-MM-dd");

            var client = new RestClient($"https://api.sunrise-sunset.org/json?lat={lat}&lng={lng}&date={dt}");

            var response = client.Execute(new RestRequest());

            var info = JsonConvert.DeserializeObject<ObjectResults>(response.Content);

            return info;
        }

        public static string ReturnStringToFormatted(string input)
        {
            var PM = input.Substring(input.Length - 2) == "PM";
            input = input.Remove(input.Length - 3);
            var split = input.Split(':');
            var splitList = split.Select(int.Parse).ToList();
            if (PM)
                splitList[0] += 13;
            else
                splitList[0] += 1;
            var result = $"{splitList[0].ToString()}:{splitList[1].ToString()}";
            return result;
        }
    }
}

