var itemCategory = $('#ItemCategory');
var itemSubcategory = $('#ItemSubcategory');
var tradeCategory = $('#TradeCategory');
var tradeSubCategory = $('#TradeSubcategory');

$(function () {
    itemCategory.ready(function () {
        var reference = parseInt(itemCategory.data("ref"));
        if (reference > 0) {
            console.log("categories ready");
            getPresetCategories(reference).then(function (result) {                
                console.log(result);
                itemCategory.val(result[0]).trigger("change");
                tradeCategory.val(result[2]).trigger("change");
                setTimeout(function () {
                    itemSubcategory.val(result[1]);
                    tradeSubCategory.val(result[3]);
                }, 1000);
            });
        }
    });
});