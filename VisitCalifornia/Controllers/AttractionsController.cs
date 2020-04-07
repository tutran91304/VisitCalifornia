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


            /*TESTING*/
            var testlocation = new Point(34.4272181, -118.5995318) { SRID = 4326 };
            var originWKT = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
            //var targetWKT = "PROJCS[\"US National Atlas Equal Area\",GEOGCS[\"Unspecified datum based upon the Clarke 1866 Authalic Sphere\",DATUM[\"Not_specified_based_on_Clarke_1866_Authalic_Sphere\",SPHEROID[\"Clarke 1866 Authalic Sphere\",6370997,0,AUTHORITY[\"EPSG\",\"7052\"]],AUTHORITY[\"EPSG\",\"6052\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4052\"]],PROJECTION[\"Lambert_Azimuthal_Equal_Area\"],PARAMETER[\"latitude_of_center\",45],PARAMETER[\"longitude_of_center\",-100],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],AUTHORITY[\"EPSG\",\"2163\"]]";
            var targetWKT = @"PROJCS[""NAD83 / UTM zone 11N"",
                        GEOGCS[""NAD83"",
                            DATUM[""North_American_Datum_1983"",
                                SPHEROID[""GRS 1980"",6378137,298.257222101,
                                    AUTHORITY[""EPSG"",""7019""]],
                                TOWGS84[0,0,0,0,0,0,0],
                                AUTHORITY[""EPSG"",""6269""]],
                            PRIMEM[""Greenwich"",0,
                                AUTHORITY[""EPSG"",""8901""]],
                            UNIT[""degree"",0.0174532925199433,
                                AUTHORITY[""EPSG"",""9122""]],
                            AUTHORITY[""EPSG"",""4269""]],
                        PROJECTION[""Transverse_Mercator""],
                        PARAMETER[""latitude_of_origin"",0],
                        PARAMETER[""central_meridian"",-117],
                        PARAMETER[""scale_factor"",0.9996],
                        PARAMETER[""false_easting"",500000],
                        PARAMETER[""false_northing"",0],
                        UNIT[""metre"",1,
                            AUTHORITY[""EPSG"",""9001""]],
                        AXIS[""Easting"",EAST],
                        AXIS[""Northing"",NORTH],
                        AUTHORITY[""EPSG"",""26911""]]";



            var coordinateSystemFactory = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
            var transformationFactory = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();

            var originCoordinateSystem = coordinateSystemFactory.CreateFromWkt(originWKT);
            var targetCoordinateSystem = coordinateSystemFactory.CreateFromWkt(targetWKT);

            var transform = transformationFactory.CreateFromCoordinateSystems(originCoordinateSystem, targetCoordinateSystem);


            
            double[] loc = new double[2];
            loc[0] = location.Y;
            loc[1] = location.X;


            double[] loc2 = new double[2];
            loc2[0] = testlocation.Y;
            loc2[1] = testlocation.X;

            var newPointA = transform.MathTransform.Transform(loc);
            var newPointB = transform.MathTransform.Transform(loc2);

            Point pointA = new Point(newPointA[1], newPointA[0]) { SRID = 2163 };
            Point pointB = new Point(newPointB[1], newPointB[0]) { SRID = 2163 };

            var distance = pointA.Distance(pointB);
            /*END OF TESTING*/



            // Fetch the tourist attractions and their
            // distances from the input location 
            // using spatial queries.
            var touristAttractions = _dbContext
                .TouristAttractions
                .Select(t => new { Place = t, Distance = t.Location.Distance(location), RealDistance = 33.33 })
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