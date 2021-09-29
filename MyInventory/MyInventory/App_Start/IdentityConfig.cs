using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MILibrary.Database.Entities;
using System.Web;

namespace MyInventory
{

    //Application Sign In Manager - Handles signing a user in and out
    public class AppSignInManager : IDisposable
    {
        private IOwinContext _context => HttpContext.Current.GetOwinContext();
        public AppSignInManager()
        {
        }

        public static AppSignInManager Create() => new AppSignInManager();

        public void SignIn(MI_USER User, bool isPersistent)
        {

            //Initialize the list of claims that we will save
            List<Claim> claims = new List<Claim>();

            //Create the claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, User.USER_ID.ToString()));
            claims.Add(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", User.USER_ID.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, User.FIRSTNAME));
            claims.Add(new Claim(ClaimTypes.Email, User.EMAIL));

            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            _context.Authentication.SignIn(new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(30)
            }, identity);
        }

        public void SignOut()
        {
            _context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        public void Dispose() { }
    }
}
