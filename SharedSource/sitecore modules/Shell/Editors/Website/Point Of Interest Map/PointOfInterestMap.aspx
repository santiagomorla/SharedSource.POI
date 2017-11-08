<%@ page language="C#" autoeventwireup="true" codebehind="PointOfInterestMap.aspx.cs" inherits="SharedSource.POI.sitecore_modules.Shell.Editors.Website.Point_Of_Interest_Map.PointOfInterestMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Point Of Interest Map</title>
    
    <!-- STYLES -->
    <link href="styles/jquery-ui.min.css" rel="stylesheet" />
    <link href="styles/jquery-ui.structure.min.css" rel="stylesheet" />
    <link href="styles/jquery-ui.theme.min.css" rel="stylesheet" />
    <link href="styles/styles.css" rel="stylesheet" />
    <link href="styles/font-awesome.min.css" rel="stylesheet" />
    
    <!-- SCRIPTS -->
    <script src="scripts/jquery-3.0.0.min.js"></script>
    <script src="scripts/jquery-ui.min.js"></script>
    <script src="scripts/jquery.validate.min.js"></script>
    <script src="scripts/jquery.validate.additional-methods.min.js"></script>
    <script src="scripts/interact.min.js"></script>
    <script src="scripts/scripts.js"></script>

</head>
<body>

    <div class="poi-map__container">
        <div class="poi-map__wrapper">

            <% var icon = "fa-map-marker"; %>

            <img src="<%= ImageUrl %>" alt="Background Image is missing" class="map-image" />

            <% if(PoiList != null) { %>
            <% foreach (var poi in PoiList)
                   { %>

            <div class="poi-map__poi poi-map__poi--no-images draggable" data-id="<%=poi.Id%>" style="top: <%=poi.Top%>%; left: <%=poi.Left%>%" data-x="<%=poi.Left/100*ImageWidth%>" data-y="<%=poi.Top/100*ImageHeight%>">

                <% if(!string.IsNullOrWhiteSpace(poi.Title)) { %>
                <h3 class="poi-map__poi-title"><%= poi.Title %></h3>
                <% } %>

                <span><a href="<%= new Sitecore.Data.ID(poi.Id)%>" class="click"><i class="fa <%= !string.IsNullOrWhiteSpace(poi.Icon) ? poi.Icon : icon %>" aria-hidden="true"></i></a></span>
            </div>

            <% } %>
            <% } %>
        </div>
    </div>

    <div id="dialogCreate" title="Create Point of Interest">

        <form id="mapsForm">
            <fieldset>

                <label for="txtTitle">Name: </label>
                <input class="form-field" type="text" name="title" id="txtTitle" />

                <label for="txtTop">Top: </label>
                <input class="form-field" type="text" name="top" id="txtTop" data-rule-required="true" data-rule-number="true" data-msg-required="Required. Enter a number." />

                <label for="txtLeft">Left: </label>
                <input class="form-field" type="text" name="left" id="txtLeft" data-rule-required="true" data-rule-number="true" data-msg-required="Required. Enter a number." />

                <input type="button" id="btnSubmit" value="Submit" />
                
                <input type="hidden" id="hdnMapId" name="hdnMapId" value="<%= MapId %>" />
                <input type="hidden" id="hdnWidth" name="hdnWidth" value="<%= ImageWidth %>" />
                <input type="hidden" id="hdnHeight" name="hdnHeight" value="<%= ImageHeight %>" />
            </fieldset>
        </form>

    </div>

    <div id="dialogInfo" title="Info">
        <p>
            <span id="spanInfo"></span>
        </p>
    </div>

</body>
</html>
