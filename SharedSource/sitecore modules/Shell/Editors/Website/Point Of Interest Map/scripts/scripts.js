var j = jQuery.noConflict();
var postUrl = "PointOfInterestMap.aspx/SavePoI";
var postUpdateUrl = "PointOfInterestMap.aspx/UpdatePoI";

//containers
var classImageMap = ".map-image";
var dialogCreate = "div#dialogCreate";
var dialogInfo = "div#dialogInfo";

var spanError = "span#error-message";
var spanInfo = "span#spanInfo";


//form fields
var txtTitle = "#txtTitle";
var txtTop = "#txtTop";
var txtLeft = "#txtLeft";

var hdnMapId = "#hdnMapId";
var hdnWidth = "#hdnWidth";
var hdnHeight = "#hdnHeight";

var formId = "#mapsForm";
var fromName = "mapsForm";

var debug = false;

function OnSuccess(response) {
    console.log('OnSuccess');
    showInfoMessage(response.d);
}
function OnFailure(response) {
    console.log(response);
    showInfoMessage(response.responseText);
}

function closeDialog(name) {
    j(name).dialog("close");
}

function openDialog(name) {
    j(name).dialog("open");

    if (j(name).attr('id') === 'dialogInfo') {
        j(name).on('dialogclose', function (event) {
            location.reload();
        });
    }

}

function showInfoMessage(message) {
    closeDialog(dialogCreate);
    j(spanInfo).html(message);
    openDialog(dialogInfo);
}

function submitData() {

    j.ajax({
        type: "POST",
        url: postUrl,
        data:
			"{" +
			"parentid: \"" + j(hdnMapId).val() + "\"," +
			"title: \"" + j(txtTitle).val() + "\"," +
			"top: \"" + j(txtTop).val() + "\"," +
			"left: \"" + j(txtLeft).val() + "\"" +
			" }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
        failure: OnFailure,
        error: OnFailure
    });
}

function updateData(id, top, left) {

    j.ajax({
        type: "POST",
        url: postUpdateUrl,
        data:
            "{" +
                "id: \"" + id + "\"," +
                "top: \"" + top + "\"," +
                "left: \"" + left + "\"" +
                " }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        failure: OnFailure,
        error: OnFailure
    });
}

//document - ready
j(document).ready(function () {
    //enable dialog
    j(dialogCreate).dialog({ autoOpen: false, closeText: "" });
    j(dialogInfo).dialog({ autoOpen: false, closeText: "" });

    //form submit
    j(formId).validate({
        submitHandler: function (form) {
            if (debug)
                console.log("valid!");
            submitData();
        },
        invalidHandler: function (event, validator) {
            if (debug)
                console.log("not valid!");
        }
    });
    j(formId).submit(function (e) {
        e.preventDefault();
    });
    j("#btnSubmit").click(function () { j(formId).submit(); });

    //capture map clicks
    j(classImageMap).click(function (e) {

        var dialogOpen = j(dialogCreate).dialog("isOpen") || j(dialogInfo).dialog("isOpen");

        if (!dialogOpen) {
            var offset = j(this).offset();
            var relativeWidth = (e.pageX - offset.left);
            var relativeHeight = (e.pageY - offset.top);

            var width = parseInt(j(hdnWidth).val());
            var height = parseInt(j(hdnHeight).val());

            var percentWidth = (relativeWidth / width) * 100;
            var percentHeight = (relativeHeight / height) * 100;

            var message = relativeWidth + ":" + relativeHeight;

            if (debug)
                console.log(message);

            j(txtTitle).val("");
            j(txtLeft).val(percentWidth.toFixed(2));
            j(txtTop).val(percentHeight.toFixed(2));

            //position dialog on mouse cursor
            j(dialogCreate).dialog("option",
                "position",
                { my: "left top", at: "left bottom", of: e, collision: "fit" });
            openDialog(dialogCreate);
        }
    });

    j(".click").on("click",
        function (e) {
            e.preventDefault();
            return false;
        });

    j(".click").on("mouseup",
        function (e) {
            e.preventDefault();
            var target = j(e.currentTarget);
            if (!target.hasClass('no-click')) {
                parent.scForm.postRequest('', '', '', 'item:load(id=' + target.attr('href') + ')');
            }
        });

    // target elements with the "draggable" class
    interact('.draggable')
		.draggable({
		    // enable inertial throwing
		    inertia: true,
		    // keep the element within the area of it's parent
		    restrict: {
		        restriction: "parent",
		        endOnly: true,
		        elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
		    },
		    // enable autoScroll
		    autoScroll: true,

		    // call this function on every dragmove event
		    onstart: function (event) {
		        var anchor = event.target.querySelector("a");
		        j(anchor).addClass('no-click');
		    },
		    onmove: dragMoveListener,
		    onend: function (event) {

		        var id = event.target.getAttribute('data-id');
		        var top = event.target.getAttribute('data-y') * 100 / j('.map-image').height();
		        var left = event.target.getAttribute('data-x') * 100 / j('.map-image').width();

		        updateData(id, top, left);

		        var anchor = event.target.querySelector("a");
		        j(anchor).removeClass('no-click');
		    }
		});
});

function dragMoveListener(event) {
    var target = event.target,
        // keep the dragged position in the data-x/data-y attributes
        x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
        y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;


    var xp = x - j(event.target).width() / 2;
    var yp = y - j(event.target).height();

    target.style.top = '';
    target.style.left = '';

    // translate the element
    target.style.webkitTransform =
        target.style.transform =
        'translate(' + xp + 'px, ' + yp + 'px)';

    // update the posiion attributes
    target.setAttribute('data-x', x);
    target.setAttribute('data-y', y);
}

// this is used later in the resizing and gesture demos
window.dragMoveListener = dragMoveListener;



