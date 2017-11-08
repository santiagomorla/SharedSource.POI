using System.Web.Mvc;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;

namespace SharedSource.POI.Controllers
{
    public class GlobalController : SitecoreController
    {
        /// <summary>
        /// Renders the point of interest map module.
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderPointOfInterestMapModule()
        {
            var model = RenderingContext.Current.Rendering.Item;
            return View("/Views/Global/PointOfInterestMapModule.cshtml", model);
        }
    }
}