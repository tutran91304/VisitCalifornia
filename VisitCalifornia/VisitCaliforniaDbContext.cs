using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using SpatialSample.Entities;

namespace VisitCalifornia
{
    public class VisitCaliforniaDbContext : DbContext
    {
        public DbSet<TouristAttraction> TouristAttractions { get; set; }

        public VisitCaliforniaDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<TouristAttraction>()
                .HasData(
                    new TouristAttraction
                    {
                        Id = 1,
                            Name = "Disney Land",
                            Location = geometryFactory.CreatePoint(new Coordinate(33.812511, -117.918976))
                    },
                    new TouristAttraction
                    {
                        Id = 2,
                            Name = "The Golden Temple of Amritsar",
                            Location = geometryFactory.CreatePoint(new Coordinate(34.4272181, -118.5995318))
                    },
                    new TouristAttraction
                    {
                        Id = 3,
                            Name = "Universal Studios",
                            Location = geometryFactory.CreatePoint(new Coordinate(34.141354, -118.352898))
                    },
                    new TouristAttraction
                    {
                        Id = 4,
                            Name = "Legoland",
                            Location = geometryFactory.CreatePoint(new Coordinate(33.1227, -117.3067))
                    },
                    new TouristAttraction
                    {
                        Id = 5,
                            Name = "Golden Gate Bridge",
                            Location = geometryFactory.CreatePoint(new Coordinate(37.8197222, -122.4788889))
                    },
                    new TouristAttraction
                    {
                        Id = 6,
                            Name = "Yosemite National Park",
                            Location = geometryFactory.CreatePoint(new Coordinate(37.865101, -119.538330))
                    }
                );
        }
    }
}