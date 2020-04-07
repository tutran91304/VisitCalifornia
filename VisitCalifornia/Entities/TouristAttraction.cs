using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpatialSample.Entities
{
    public class TouristAttraction
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "geometry")]
        public Point Location { get; set; }
    }
}