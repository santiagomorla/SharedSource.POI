using System;

namespace SharedSource.POI.sitecore_modules.Shell.Editors.Website.Point_Of_Interest_Map.Models
{
    public class Poi
    {
        public string Title { get; set; }
        public decimal Top { get; set; }
        public decimal Left { get; set; }
        public string Icon { get; set; }
        public Guid Id { get; set; }
    }
}