using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Collections.Generic;
using GeoProximityMVC.Controllers;
using GeoProximityMVC.Models.DTO;
using GeoProximityMVC.Models.DAL;
using GeoProximityMVC.Service;
using Moq;

namespace GeoProximityMVCTest
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        ///TODO: Need to work on this
        public async Task GetUserTestWithNoLocationUsingMock()
        {
            var userObjs = JsonConvert.DeserializeObject<dynamic>("[{\"id\":1,\"name\":\"Leanne Graham\",\"username\":\"Bret\",\"email\":\"Sincere@april.biz\",\"address\":{\"street\":\"Kulas Light\",\"suite\":\"Apt. 556\",\"city\":\"Gwenborough\",\"zipcode\":\"92998-3874\",\"geo\":{\"lat\":\"-37.3159\",\"lng\":\"81.1496\"}},\"phone\":\"1-770-736-8031 x56442\",\"website\":\"hildegard.org\",\"company\":{\"name\":\"Romaguera-Crona\",\"catchPhrase\":\"Multi-layered client-server neural-net\",\"bs\":\"harness real-time e-markets\"}},{\"id\":2,\"name\":\"Ervin Howell\",\"username\":\"Antonette\",\"email\":\"Shanna@melissa.tv\",\"address\":{\"street\":\"Victor Plains\",\"suite\":\"Suite 879\",\"city\":\"Wisokyburgh\",\"zipcode\":\"90566-7771\",\"geo\":{\"lat\":\"-43.9509\",\"lng\":\"-34.4618\"}},\"phone\":\"010-692-6593 x09125\",\"website\":\"anastasia.net\",\"company\":{\"name\":\"Deckow-Crist\",\"catchPhrase\":\"Proactive didactic contingency\",\"bs\":\"synergize scalable supply-chains\"}}]");
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(ur => ur.GetUsersAsync()).Returns(Task.FromResult(userObjs));
            
            Tuple<double, double> geocode = new Tuple<double, double>(49.282730, -123.120735);
            var geocodeServiceMock = new Mock<IGeocodeService>();
            geocodeServiceMock.Setup(gs => gs.GetGeoCoordinatesOfLocationAsync("Vancouver"))
                .Returns(Task.FromResult(geocode));

            UserController uc = new UserController(userRepositoryMock.Object, geocodeServiceMock.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            
            var jsonResult = await uc.GetUsersOrderByDistanceAsync("Vancouver");

            var response = jsonResult as ResponseMessageResult;
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task GetUserTestWithNoLocation()
        {
            UserController uc = new UserController(new UserRepository(), new GoogleGeocodeService())
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var jsonResult = await uc.GetUsersOrderByDistanceAsync(string.Empty);

            var response = jsonResult as ResponseMessageResult;
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task GetUserTestHasRequiredColumns()
        {
            UserController uc = new UserController(new UserRepository(), new GoogleGeocodeService())
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var jsonResult = await uc.GetUsersOrderByDistanceAsync("Vancouver");
            var response = jsonResult as OkNegotiatedContentResult<List<UserDTO>>;
            Assert.IsNotNull(response.Content);
        }

        [TestMethod]
        public async Task GetUserTestHasDataSortedByDistanceAsc()
        {
            UserController uc = new UserController(new UserRepository(), new GoogleGeocodeService())
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var jsonResult = await uc.GetUsersOrderByDistanceAsync("Vancouver");
            var response = jsonResult as OkNegotiatedContentResult<List<UserDTO>>;

            double lastDist = response.Content[0].Distance;
            foreach (var user in response.Content)
            {
                double dist = user.Distance;
                Assert.IsTrue(lastDist <= dist, "Data is not Ordered");
                lastDist = dist;
            }
        }
    }
}
