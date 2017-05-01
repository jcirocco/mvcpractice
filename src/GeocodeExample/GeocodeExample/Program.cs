using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace GeocodeExample
{
    class Program
    {

        private static string apiKey = "AIzaSyCAXrX3yZN2lqlSroE3aSrBvalgCPqcpq0";

        static void Main(string[] args)
        {

            Console.WriteLine(@"This program calls out to the google geocoding API. Please fill in the following prompts to to return latitude and longitude coordinates");

            Console.WriteLine("\n");

            Console.WriteLine("Please enter a state: ");
            string state = Console.ReadLine();

            Console.WriteLine("\n");

            Console.WriteLine("Please enter a city: ");
            string city = Console.ReadLine();

            Console.WriteLine("\n");

            Console.WriteLine("Please enter an address: ");
            string address = Console.ReadLine();

            string fullAddress = String.Format("{0}, {1}, {2}", address, city, state);

            Console.WriteLine("\n Sending out geocoded request now! \n");

            // Create URL
            // Example format: https://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=YOUR_API_KEY
            StringBuilder queryString = new StringBuilder();

            // Add Address Components
            queryString.Append("address=");
            queryString.Append(Uri.EscapeUriString(fullAddress));

            // Add API Key
            queryString.Append(String.Format("&key={0}",apiKey));

            // Build out URI
            UriBuilder uriBuilder = new UriBuilder("https://maps.googleapis.com");
            uriBuilder.Path = "/maps/api/geocode/json";
            uriBuilder.Query = queryString.ToString();

            // Build out the HTTP Request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "GET";


            //var result = new Result();
            //var serializer = new JsonSerializer();
            string ret = "";
            double lat;
            double lng;

            using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))       
            {

                //string res = sr.ReadToEnd();
                //Console.WriteLine(res);
                Result results = DeserializeResults(sr);
                //result = serializer.Deserialize<Result>(jsonReader);
                ret = results.formatted_address;
                lat = results.geometry.location.lat;
                lng = results.geometry.location.lng;
                //result = serializer.Deserialize<Result>(jsonReader);

            }

            Console.WriteLine(String.Format("Latitude: {0}",lat));
            Console.WriteLine(String.Format("Longitudee: {0}", lng));

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

        }

        public static Result DeserializeResults(StreamReader sr)
        {
            var root = new RootObject();
            var serializer = new JsonSerializer();
            using (var jsonReader = new JsonTextReader(sr))
            {
                root = serializer.Deserialize<RootObject>(jsonReader);
            }

            var result = root.results[0];

            return result;
        }

        public class AddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public List<string> types { get; set; }
        }

        public class Bounds
        {
            public Location northeast { get; set; }
            public Location southwest { get; set; }
        }

        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Geometry
        {
            public Bounds bounds { get; set; }
            public Location location { get; set; }
            public string location_type { get; set; }
            public Bounds viewport { get; set; }
        }

        public class Result
        {
            public List<AddressComponent> address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public bool partial_match { get; set; }
            public List<string> types { get; set; }
        }

        public class RootObject
        {
            public List<Result> results { get; set; }
            public string status { get; set; }
        }

    }
}
