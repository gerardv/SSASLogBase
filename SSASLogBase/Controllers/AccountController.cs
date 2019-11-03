using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SSASLogBase.Controllers
{
    [Route("[controller]/[action]")] // Not sure about this...
    public class AccountController : Controller
    {
        // This first action may be needed for deep links, redirecting to the requested resource after being challenged successfully.
        //[HttpGet]
        //public IActionResult SignIn()
        //{
        //    var redirectUrl = Url.Action(nameof(RefreshesController.Index), "Home");
        //    return Challenge(
        //        new AuthenticationProperties { RedirectUri = redirectUrl, AllowRefresh = true },
        //        OpenIdConnectDefaults.AuthenticationScheme);
        //}

        [HttpGet]
        public IActionResult SignOut()
        {
            // Remove all cache entries for this user and send an OpenID Connect sign-out request.
            string userObjectID = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var authContext = new AuthenticationContext(AzureAdOptions.Settings.Authority,
                                                        new NaiveSessionCache(userObjectID, HttpContext.Session));
            authContext.TokenCache.Clear();

            //Let Azure AD sign-out
            var callbackUrl = Url.Action("Index", "Refreshes", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl, AllowRefresh = true },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}
