var itemSearch = $("#searchBoxItem");
var itemButton = $("#searchButtonItem");
var itemTable = $("#itemTable");
var accountSearch = $("#searchBoxAccount");
var accountButton = $("#searchButtonAccount");
var accountTable = $("#accountTable");
var searchPreviousItem = "";
var searchPreviousAccount = "";

function getItemReports() {
    var searchText = itemSearch.val();
    searchPreviousItem = searchText;
    getReportSearch(itemTable, searchText, "/Ajax/SearchReportedItems");
}

function getAccountReports() {
    var searchText = accountSearch.val();
    searchPreviousAccount = searchText;
    getReportSearch(accountTable, searchText, "/Ajax/SearchReportedAccounts");
}

function getReportsRefresh(type) {
    switch (type) {
        case 0:
            getReportSearch(itemTable, searchPreviousItem, "/Ajax/SearchReportedItems");
        break;
        case 1:
            getReportSearch(accountTable, searchPreviousAccount, "/Ajax/SearchReportedAccounts");
        break;
    }
}

function setClearReportable(id) {
    setClearAjax(id).then(function (data) {
        getReportsRefresh(data);
    });
}

function setRemoveReportable(id) {
    setRemoveAjax(id).then(function (data) {
        getReportsRefresh(data);
    });
}

$(function() {
    itemButton.click(function () {
        getItemReports();
    });
    accountButton.click(function () {
        getAccountReports();
    });
});