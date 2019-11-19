using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoProximityMVC.Models.DTO;

namespace GeoProximityMVC.Models.DAL
{
    public interface IUserRepository
    {
        Task<dynamic> GetUsersAsync();
        IList<UserDTO> GetUserOrderedByDistance(double lat, double lng);
    }
}
