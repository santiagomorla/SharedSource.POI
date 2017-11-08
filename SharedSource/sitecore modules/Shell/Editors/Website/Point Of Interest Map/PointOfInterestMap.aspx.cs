using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Services;
using SharedSource.POI.sitecore_modules.Shell.Editors.Website.Point_Of_Interest_Map.Models;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace SharedSource.POI.sitecore_modules.Shell.Editors.Website.Point_Of_Interest_Map
{
    public partial class PointOfInterestMap : System.Web.UI.Page
    {
        public string ImageUrl { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string MapId { get; set; }
        public IEnumerable<Poi> PoiList { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            MapId = Request.QueryString[Constants.QueryString.Id];
            if (string.IsNullOrEmpty(MapId)) return;

            var item = Helpers.SitecoreHelper.GetItemFromGuidInMaster(MapId);
            if (item == null) return;

            var poiList = new List<Poi>();
            foreach (Item poiItem in item.Children)
            {
                var title = poiItem.Fields[Constants.Fields.PoiTitle].Value;

                var poi = new Poi
                {
                    Title = title,
                    Top = System.Convert.ToDecimal(poiItem.Fields[Constants.Fields.PoiTop].Value),
                    Left = System.Convert.ToDecimal(poiItem.Fields[Constants.Fields.PoiLeft].Value),
                    Icon = poiItem.Fields[Constants.Fields.PoiIcon].Value,
                    Id = poiItem.ID.Guid
                };

                if (Sitecore.Configuration.Settings.GetBoolSetting("SharedSource.Poi.ShowTitleFallback", true) && string.IsNullOrWhiteSpace(title))
                {
                    poi.Title = poiItem.Name;
                }

                poiList.Add(poi);
            }

            ImageField image = item.Fields[Constants.Fields.MapBackgroundImage];
            if (image?.MediaItem == null) return;

            ImageUrl = MediaManager.GetMediaUrl(image.MediaItem);
            ImageHeight = System.Convert.ToInt32(image.Height);
            ImageWidth = System.Convert.ToInt32(image.Width);
            PoiList = poiList;
        }

        [WebMethod]
        public static string SavePoI(string parentid, string title, string top, string left)
        {
            Database database = Factory.GetDatabase("master");
            string successMessage = Sitecore.Globalization.Translate.Text("Point Of Interest Created");
            string nameExistsMessage = Sitecore.Globalization.Translate.Text("Point Of Interest Duplicated");

            try
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {

                    using (new DatabaseSwitcher(database))
                    {
                        Item parent = Helpers.SitecoreHelper.GetItemFromGuidInMaster(parentid);

                        var name = title;
                        if (string.IsNullOrWhiteSpace(title))
                            name = $"T{top} L{left}";

                        name = ItemUtil.ProposeValidItemName(name);

                        var nameExists = parent.Children != null && parent.Children.Any() &&
                                         parent.Children.Any(
                                             x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                        if (nameExists)
                        {
                            return nameExistsMessage;
                        }

                        TemplateID templateId = new TemplateID(new ID(Constants.TemplateId.PointOfInterest));
                        Item newItem = parent.Add(name, templateId);

                        try
                        {

                            newItem.Editing.BeginEdit();
                            using (new EditContext(newItem))
                            {
                                if (Sitecore.Configuration.Settings.GetBoolSetting("SharedSource.Poi.UseTitle", true))
                                {
                                    newItem.Fields[Constants.Fields.PoiTitle].Value = title;
                                }
                                
                                newItem.Fields[Constants.Fields.PoiTop].Value = Math.Round(Convert.ToDecimal(top), 2).ToString(CultureInfo.InvariantCulture);
                                newItem.Fields[Constants.Fields.PoiLeft].Value = Math.Round(Convert.ToDecimal(left), 2).ToString(CultureInfo.InvariantCulture);
                            }
                            newItem.Editing.EndEdit(false, true);
                            newItem.Editing.AcceptChanges();
                        }
                        catch (Exception e)
                        {
                            newItem.Editing.CancelEdit();
                            newItem.Delete();
                            Sitecore.Diagnostics.Log.Error(e.Message, e);
                            return "Error: " + e.Message;
                        }

                        return successMessage;
                    }
                }
            }
            catch (Exception e)
            {
                Sitecore.Diagnostics.Log.Error(e.Message, e);
                return "Error: " + e.Message;
            }
        }

        [WebMethod]
        public static string UpdatePoI(string id, string top, string left)
        {
            Database database = Factory.GetDatabase("master");
            
            try
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    using (new DatabaseSwitcher(database))
                    {
                        Item item = Helpers.SitecoreHelper.GetItemFromGuidInMaster(id);

                        if (item != null )
                        {
                            try
                            {

                                item.Editing.BeginEdit();
                                using (new EditContext(item))
                                {
                                    item.Fields[Constants.Fields.PoiTop].Value = Math.Round(Convert.ToDecimal(top), 2).ToString(CultureInfo.InvariantCulture);
                                    item.Fields[Constants.Fields.PoiLeft].Value = Math.Round(Convert.ToDecimal(left), 2).ToString(CultureInfo.InvariantCulture);
                                }
                                item.Editing.EndEdit(false, true);
                                item.Editing.AcceptChanges();
                            }
                            catch (Exception e)
                            {
                                item.Editing.CancelEdit();
                                Sitecore.Diagnostics.Log.Error(e.Message, e);
                                return "Error: " + e.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Sitecore.Diagnostics.Log.Error(e.Message, e);
                return "Error: " + e.Message;
            }

            return string.Empty;
        }
    }
}