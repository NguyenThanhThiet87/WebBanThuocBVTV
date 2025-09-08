using Google.Api.Gax.Grpc;
using Google.Api.Gax.Grpc.Rest;
using Google.Maps.Routing.V2;
using Google.Type;
using Microsoft.AspNetCore.Mvc;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GoogleMapController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        //public IActionResult Road()
        //{
        //    //RoutesClient client = RoutesClient.Create();
        //    //CallSettings callSettings = CallSettings.FromHeader("X-Goog-FieldMask", "*");
        //    //ComputeRoutesRequest request = new ComputeRoutesRequest
        //    //{
        //    //    Origin = new Waypoint
        //    //    {
        //    //        Location = new Location { LatLng = new LatLng { Latitude = 10.007799339534555, Longitude = 105.72295611038072 } }
        //    //    },
        //    //    Destination = new Waypoint
        //    //    {
        //    //        Location = new Location { LatLng = new LatLng { Latitude = 9.981310036411275, Longitude = 105.09186288358553 } }
        //    //    },
        //    //    TravelMode = RouteTravelMode.Drive,
        //    //    RoutingPreference = RoutingPreference.TrafficAware
        //    //};
        //    //ComputeRoutesResponse response = client.ComputeRoutes(request, callSettings);
        //    //Console.WriteLine(response);
        //    //// Trả về view Google Map
        //    //return View("GoogleMap");
        //}
    }
}
