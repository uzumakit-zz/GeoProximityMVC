using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using GeoProximityMVC.Models.DTO;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using GeoProximityMVC.Utils;
using GeoProximityMVC.Helpers;

namespace GeoProximityMVC.Models.DAL
{
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Get User rows sorted by distance where closest to location is on top and furthest in the last.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public async Task<dynamic> GetUsersAsync()
        {
            try
            {
                //string data = CacheUtil.Get(Constants.CACHE_USER_KEY);
                //if (data != null) return data;

                string uri = ConfigurationManager.AppSettings["UserDataService"] ?? string.Empty;
                using (HttpClient client = new HttpClient())
                {
                    var userJson = await client.GetStringAsync(uri);
                    var userobjs = JsonConvert.DeserializeObject<dynamic>(userJson);
                    
                    //CacheUtil.Add(Constants.CACHE_USER_KEY, userJson, 30);

                    return userobjs;
                }
            }
            catch (Exception ex)
            {
                //Log exception for troubleshooting

                //Throw exception for upper layer to handle
                throw new Exception("Unable to get the user data");
            }
        }

        /// <summary>
        /// Get User rows sorted by distance where closest to location is on top and furthest in the last.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public IList<UserDTO> GetUserOrderedByDistance(double lat, double lng)
        {
            SortedDictionary<double, List<UserDTO>> sortedDictOfUsers = new SortedDictionary<double, List<UserDTO>>();

            try
            {
                string uri = ConfigurationManager.AppSettings["UserDataService"] ?? string.Empty;
                using (HttpClient client = new HttpClient())
                {
                    var userJson = client.GetStringAsync(uri).Result;
                    var userobjs = JsonConvert.DeserializeObject<dynamic>(userJson);

                    foreach (var obj in userobjs)
                    {
                        var userLatLng = Tuple.Create(Convert.ToDouble(obj.address.geo.lat.Value),
                            Convert.ToDouble(obj.address.geo.lng.Value));
                        var locLatLng = Tuple.Create(lat, lng);

                        double distance = GeoProximityMVC.Utils.GeocodeUtil.GetDistance(userLatLng, locLatLng);

                        UserDTO userDTO = new UserDTO
                        {
                            Name = obj.name,
                            Address = obj.address.suite + ' ' + obj.address.street + ' ' + obj.address.city + ' ' + obj.address.zipcode,
                            Company = obj.company.name,
                            Phone = obj.phone,
                            Distance = distance
                        };

                        //TODO : Eliminate search by using DS capable of handling duplicate keys. 
                        // Cache data for future use.
                        if (sortedDictOfUsers.ContainsKey(userDTO.Distance))
                        {
                            List<UserDTO> tempUserList = sortedDictOfUsers[userDTO.Distance];
                            tempUserList.Add(userDTO);
                            sortedDictOfUsers[userDTO.Distance] = tempUserList;
                        }
                        else
                        {
                            List<UserDTO> tempUserList = new List<UserDTO>();
                            tempUserList.Add(userDTO);
                            sortedDictOfUsers.Add(userDTO.Distance, tempUserList);
                        }
                    }
                }

                List<UserDTO> listOfUserDTO = new List<UserDTO>();
                foreach (KeyValuePair<double, List<UserDTO>> userDTOObjs in sortedDictOfUsers)
                {
                    foreach (UserDTO userDTOObj in userDTOObjs.Value)
                    {
                        listOfUserDTO.Add(userDTOObj);
                    }
                }

                return listOfUserDTO;
            }
            catch (Exception ex)
            {
                //Log exception for troubleshooting

                //Throw exception for upper layer to handle
                throw new Exception("Unable to get the user data based on given location");
            }

        }
    }
}