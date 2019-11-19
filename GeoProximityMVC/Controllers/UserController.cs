using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using GeoProximityMVC.Models.DTO;
using GeoProximityMVC.Models.DAL;
using GeoProximityMVC.Utils;
using GeoProximityMVC.Helpers;
using GeoProximityMVC.Service;

namespace GeoProximityMVC.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        IUserRepository _userRepository;
        IGeocodeService _geocodeService;
        
        public UserController(IUserRepository userRepository, IGeocodeService geocodeService)
        {
            _userRepository = userRepository;
            _geocodeService = geocodeService;
        }        

        /// <summary>
        /// Get User records with Distance, Name, Phone, Company, Address
        /// </summary>
        /// <param name="location"></param>
        /// <returns>Json</returns>
        [HttpGet]
        [Route("{loc: string}")]
        public async Task<IHttpActionResult> GetUsersOrderByDistanceAsync(string loc)
        {
            try
            {
                Task<Tuple<double, double>> geocodeTask = _geocodeService.GetGeoCoordinatesOfLocationAsync(loc);

                Task<dynamic> userJsonTask = _userRepository.GetUsersAsync();

                var userobjs = await userJsonTask;

                SortedDictionary<double, UserDTO> sortedDictOfUsers = new SortedDictionary<double, UserDTO>
                    (new DuplicateKeyComparer<double>());

                foreach (var obj in userobjs)
                {
                    var userLatLng = Tuple.Create(Convert.ToDouble(obj.address.geo.lat.Value),
                        Convert.ToDouble(obj.address.geo.lng.Value));

                    Tuple<double, double> geocode = await geocodeTask;
                    var locLatLng = Tuple.Create(geocode.Item1, geocode.Item2);

                    double distance = GeoProximityMVC.Utils.GeocodeUtil.GetDistance(userLatLng, locLatLng);

                    UserDTO userDTO = new UserDTO
                    {
                        Name = obj.name,
                        Address = obj.address.suite + ' ' + obj.address.street + ' ' + obj.address.city + ' ' + obj.address.zipcode,
                        Company = obj.company.name,
                        Phone = obj.phone,
                        Distance = distance
                    };

                    //TODO : Cache data for future use.
                    sortedDictOfUsers.Add(userDTO.Distance, userDTO);
                }

                return Ok(sortedDictOfUsers.Values.ToList());
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}
