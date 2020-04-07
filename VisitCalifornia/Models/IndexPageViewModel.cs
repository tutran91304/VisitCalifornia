using System.Collections.Generic;

namespace VisitCalifornia.Models
{
    public class AttractionViewModel
    {
        public SearchInputModel SearchInput { get; set; }
        public List<TouristAttractionViewModel> TouristAttractions { get; set; }
    }
}