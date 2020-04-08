using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using VisitCalifornia.Extensions;
using VisitCalifornia.Models;

namespace VisitCalifornia.Controllers
{
    public class AttractionsController : Controller
    {
        VisitCaliforniaDbContext _dbContext;

        public AttractionsController(VisitCaliforniaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search([FromForm] AttractionViewModel indexModel)
        {



            var indexViewModel = new AttractionViewModel
            {
                SearchInput = indexModel.SearchInput
            };

            // Convert the input latitude and longitude to a Point
            var location = new Point(indexModel.SearchInput.Latitude, indexModel.SearchInput.Longitude) { SRID = 4326 };


            var RealLocation = new Point(location.Y, location.X) { SRID = 4326 };


            // Fetch the tourist attractions and their
            // distances from the input location 
            // using spatial queries.
            var touristAttractions = _dbContext
                .TouristAttractions
                .Select(t => new { Place = t, Distance = t.Location.Distance(location), RealDistance = new Point(t.Location.Y, t.Location.X) { SRID = 4326 }.ProjectTo(2163).Distance(RealLocation.ProjectTo(2163)) })
                .ToList();

            // Ordering the result in the ascending order of distance
            indexViewModel.TouristAttractions = touristAttractions
                .OrderBy(x => x.Distance)
                .Select(t => new TouristAttractionViewModel
                {
                    Distance = Math.Round(t.Distance, 6),
                    Latitude = t.Place.Location.X,
                    Longitude = t.Place.Location.Y,
                    Name = t.Place.Name,
                    RealDistance = Math.Round(t.RealDistance, 6),
                }).ToList();

            return View("Index", indexViewModel);
        }
    }
}