using System.Web.Mvc;

namespace Exchaggle.Attributes
{
    public class AccessAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
        }
    }
}