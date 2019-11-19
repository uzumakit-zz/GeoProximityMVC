using System;
using System.Threading.Tasks;

namespace GeoProximityMVC.Service
{
    public interface IGeocodeService
    {
        Task<Tuple<double, double>> GetGeoCoordinatesOfLocationAsync(string location);
    }
}
