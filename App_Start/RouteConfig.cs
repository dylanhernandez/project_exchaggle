using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Exchaggle
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Edit",
                url: "Items/Edit/{reference}",
                defaults: new { controller = "Items", action = "Edit", reference = 0 }
            );
            routes.MapRoute(
                name: "TradeDetails",
                url: "Trades/Details/{referenceA}/{referenceB}",
                defaults: new { controller = "Trades", action = "Details", referenceA = 0, referenceB = 0 }
            );
            routes.MapRoute(
                name: "Reports",
                url: "Report/{action}/{reference}",
                defaults: new { controller = "Report", action = "Item", reference = 0 }
            );
            routes.MapRoute(
                name: "Details",
                url: "{controller}/Details/{reference}",
                defaults: new { controller = "Items", action = "Details", reference = 0 }
            );
            routes.MapRoute(
                name: "UserOffers",
                url: "Trades/UserOffers/{reference}",
                defaults: new { controller = "Trades", action = "UserOffers", reference = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "MakeOffer",
                url: "Trades/MakeOffer/{reference}",
                defaults: new { controller = "Trades", action = "MakeOffer", reference = 0 }
            );
            routes.MapRoute(
                name: "MakeOfferConfirm",
                url: "Trades/MakeOfferConfirm/{referenceA}/{referenceB}",
                defaults: new { controller = "Trades", action = "MakeOfferConfirm", referenceA = 0, referenceB = 0 }
            );
            routes.MapRoute(
                name: "AddToWishlist",
                url: "Items/AddToWishlist/{reference}",
                defaults: new { controller = "Items", action = "AddToWishlist", reference = 0 }
            );
            routes.MapRoute(
                name: "Results",
                url: "Search/Result/{reference}",
                defaults: new { controller = "Search", action = "Result", reference = -1 }
            );
            routes.MapRoute(
                name: "Main",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
