using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SharedSource.POI.Helpers
{
    public class SitecoreHelper
    {
        public static Item GetItemFromGuidInMaster(string guid)
        {
            Item obj = (Item)null;
            if (!string.IsNullOrEmpty(guid))
                obj = Factory.GetDatabase("master").GetItem(ID.Parse(guid));
            return obj;
        }
    }
}