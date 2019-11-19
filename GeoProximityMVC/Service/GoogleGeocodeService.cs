using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using GeoProximityMVC.Utils;

namespace GeoProximityMVC.Service
{
    public class GoogleGeocodeService : IGeocodeService
    {
        /// <summary>
        /// Call Google Maps API to geocode given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<Tuple<double, double>> GetGeoCoordinatesOfLocationAsync(string location)
        {
            if (string.IsNullOrEmpty(location)) throw new ArgumentException("Location is not provided");

            //Tuple<double, double> data = await CacheUtil.Get(location);
            //if (data != null) return data;

            string key = ConfigurationManager.AppSettings["GMapsAPIKey"] ?? string.Empty;
            string uri = ConfigurationManager.AppSettings["GMapsAPIURI"] ?? string.Empty;
            uri += "address=" + location + "&key=" + key;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var geocodeJson = await client.GetStringAsync(uri);
                    var geocodeObj = JsonConvert.DeserializeObject<dynamic>(geocodeJson);
                    double lat = geocodeObj.results[0].geometry.location.lat.Value;
                    double lng = geocodeObj.results[0].geometry.location.lng.Value;

                    //data = new Tuple<double, double>(lat, lng);
                    //CacheUtil.Add(location, data);
                    return new Tuple<double, double>(lat, lng);
                }
            }
            catch (Exception ex)
            {
                //data = new Tuple<double, double>(49.282730, -123.120735);
                //CacheUtil.Add(location, data);
                //return data;
                return new Tuple<double, double>(49.282730, -123.120735);
                //Log exception for troubleshooting

                //Throw exception for upper layer to handle
                //throw new Exception("Unable to geocode given location");
            }
        }
    }
}