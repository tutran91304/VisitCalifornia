﻿using NetTopologySuite;
using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Collections.Generic;

namespace VisitCalifornia.Extensions
{
    static class GeometryExtensions
    {
        static readonly NtsGeometryServices _geometryServices = NtsGeometryServices.Instance;
        static readonly CoordinateSystemServices _coordinateSystemServices
            = new CoordinateSystemServices(
                new CoordinateSystemFactory(),
                new CoordinateTransformationFactory(),
                new Dictionary<int, string>
                {
                    // Coordinate systems:

                    // (3857 and 4326 included automatically)


                    // This coordinate system covers the area of our data.
                    // Different data requires a different coordinate system.
                    [4326] = @"GEOGCS[""WGS 84"",
                        DATUM[""WGS_1984"",
                            SPHEROID[""WGS 84"",6378137,298.257223563,
                                AUTHORITY[""EPSG"",""7030""]],
                            AUTHORITY[""EPSG"",""6326""]],
                        PRIMEM[""Greenwich"",0,
                            AUTHORITY[""EPSG"",""8901""]],
                        UNIT[""degree"",0.0174532925199433,
                            AUTHORITY[""EPSG"",""9122""]],
                        AUTHORITY[""EPSG"",""4326""]]
                    ",
                    [2163] = @"PROJCS[""NAD83 / UTM zone 11N"",
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
                        AUTHORITY[""EPSG"",""26911""]]"
        });

        public static Geometry ProjectTo(this Geometry geometry, int srid)
        {
            var geometryFactory = _geometryServices.CreateGeometryFactory(srid);
            var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

            return Transform(geometry, transformation.MathTransform);
        }

        public static Geometry Transform(this Geometry geometry, MathTransform mathTransform)
        {
            geometry = geometry.Copy();
            geometry.Apply(new MathTransformFilter(mathTransform));
            return geometry;
        }

        private sealed class MathTransformFilter : ICoordinateSequenceFilter
        {
            private readonly MathTransform _mathTransform;

            public MathTransformFilter(MathTransform mathTransform)
                => _mathTransform = mathTransform;

            public bool Done => false;
            public bool GeometryChanged => true;

            public void Filter(CoordinateSequence seq, int i)
            {
                var (x, y, z) = _mathTransform.Transform(seq.GetX(i), seq.GetY(i), seq.GetZ(i));
                seq.SetX(i, x);
                seq.SetY(i, y);
                seq.SetZ(i, z);
            }
        }
    }
}