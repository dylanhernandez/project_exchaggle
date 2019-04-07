function getPresetCategories(value)
{
    return new Promise(function (resolve, reject) {
        var ajaxUrl = "/Ajax/PresetCategoryJson";
        $.ajax({
            url: ajaxUrl,
            data: { reference: value },
            cache: false,
            type: "POST",
            success: function (data) {
                resolve(data);
            }
        });
    });
}

function getSubcategories(selector, value)
{
    var ajaxUrl = "/Ajax/SubcategoriesJson";
    var processingMessage = "<option value='0'> Please wait...</option>";
    var selectorObject = $("#" + selector);
    selectorObject.html(processingMessage).show();
    $.ajax({
        url: ajaxUrl,
        data: { reference: value },
        cache: false,
        type: "POST",
        success: function (data) {
            var markup = "";
            for (var x in data) {
                markup += "<option value='" + data[x].Value + "'>" + data[x].Text + "</option>";
            }
            selectorObject.html(markup).show();
        }
    });
}

function setNotificationClear(selector, value) {
    var ajaxUrl = "/Ajax/NotificationClear";
    var selectorObject = (value > 0) ? $("#" + selector + "_" + value) : $("#" + selector);
    $.ajax({
        url: ajaxUrl,
        data: { reference: value },
        cache: false,
        type: "POST",
        success: function (data) {
            if (data === true)
            {
                var emptyHtml = "<tr><td><small>No Available Notifications</small><br />You have no available notifications at this time.</td><td></td></tr>";
                if (value > 0) {
                    selectorObject.remove();
                    if (!$("tbody#notebody > tr").length)
                    {
                        $("tbody#notebody").html(emptyHtml);
                    }
                }
                else {
                    selectorObject.html(emptyHtml);
                }
            }
        }
    });
}

function getReportSearch(selector, value, url) {
    var ajaxUrl = url;
    var selectorObject = selector;
    $.ajax({
        url: ajaxUrl,
        data: { search: value },
        cache: false,
        type: "POST",
        success: function (data) {
            var tableBuild = "<tr><td colspan='5'>Enter a search query for records</td></tr>";
            if (data !== null) {
                if (data.length > 0) {
                    tableBuild = "";
                    data.map(function (result) {
                        var row = "";
                        row += "<tr>";
                        row += "<td>" + result.ReferenceId + "</td>";
                        row += "<td>" + result.ReferenceName + "</td>";
                        row += "<td>" + result.PostedName + "</td>";
                        row += "<td>" + result.Details + "</td>";
                        row += "<td>";
                        row += "<button type='button' class='btn btn-block btn-exchaggle-secondary' role='button' style='margin-bottom:5px;' onclick='setClearReportable(" + result.ReportId + ")'>Clear Status</button>";
                        row += "<button type='button' class='btn btn-block btn-exchaggle-secondary' role='button' onclick='setRemoveReportable(" + result.ReportId + ")'>Remove</button>";
                        row += "</td></tr>";
                        tableBuild += row;
                    });
                }
            }
            selector.html(tableBuild);
        }
    });
}

function setClearAjax(reportableId) {
    return new Promise(function (resolve, reject) {
        var ajaxUrl = "/Ajax/ReportClearStatus";
        $.ajax({
            url: ajaxUrl,
            data: { reportId: reportableId },
            cache: false,
            type: "POST",
            success: function (data) {
                resolve(data);
            },
            error: function (data) {
                reject(data);
            }
        });
    });
}

function setRemoveAjax(reportableId) {
    return new Promise(function (resolve, reject) {
        var ajaxUrl = "/Ajax/ReportRemoveObject";
        $.ajax({
            url: ajaxUrl,
            data: { reportId: reportableId },
            cache: false,
            type: "POST",
            success: function (data) {
                resolve(data);
            },
            error: function (data) {
                reject(data);
            }
        });
    });
}