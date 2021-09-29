using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MILibrary.Database;
using MILibrary.Database.Entities;
using MILibrary.Database.Extensions;
using MILibrary.Password;
using MyInventory.Areas.UserManagement.Models;


namespace MyInventory.Areas.UserManagement.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        private AppDbContext _dbContext;
        private AppSignInManager _signInManager;

        public AppDbContext DBContext
        {
            get { return _dbContext ?? HttpContext.GetOwinContext().Get<AppDbContext>(); }
        }
        public AppSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>(); }
        }

        //Stub
        public UserController() { }
        //If DI was used
        public UserController(AppDbContext DBContext, AppSignInManager SignInManager)
        {
            _dbContext = DBContext;
            _signInManager = SignInManager;
        }

        // GET: UserManagement/User
        public ActionResult Index()
        {
            //Look up the currently logged in user
            int userID;
            string claimsUserID = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(claimsUserID) || !int.TryParse(claimsUserID, out userID))
            {
                //Did not get a valid value back from the claims. Something is wrong there, so just sign out the user and take them back to the sign in page.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the data from the database
            MI_USER user = DBContext.MI_USER.FirstOrDefault(x => x.USER_ID == userID);
            if (user == null)
            {
                //Failed to load the data from the databse. We have to assume that the ID was invalid, so sign out the user and make them sign in again.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Convert the data to the view model
            IndexViewModel model = new IndexViewModel()
            {
                FirstName = user.FIRSTNAME,
                LastName = user.LASTNAME,
                Email = user.EMAIL,
                CreatedDTTM = user.CREATEDDTTM
            };

            //Return the view
            return View(model);
        }

        // GET: UserManagement/User/SignIn
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: UserManagement/User/SignIn
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInViewModel model, string returnUrl)
        {
            //Validate that the model state is good
            if (!ModelState.IsValid)
                return View();

            //Look up the active status ref
            MI_STATUS_REF status = DBContext.MI_STATUS_REF.FirstOrDefault(x => x.STATUS == "Active");

            //Look up the user
            MI_USER user = DBContext.MI_USER.Where(x => x.STATUS_ID == status.STATUS_ID).FirstOrDefault(x => x.EMAIL == model.Email);
            if (user == null)
            {
                //There is no account associated with this email address
                ModelState.AddModelError("", "Either the username or password was incorrect.");
                return View();
            }

            //Validate the password.
            if (!user.ValidatePassword(DBContext, model.Password))
            {
                //The password was invalid
                ModelState.AddModelError("", "Either the username or password was incorrect.");
                return View();
            } else
            {
                //The password was valid. Sign in the user
                SignInManager.SignIn(user, model.RememberMe);

                //If there is a return URL, try to return to that.
                //Otherwise, go to the default warehouse view
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToLocal(returnUrl);
                } else
                {
                    return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
                }
            }

        }

        // POST: UserManagement/User/SignOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            //Sign out the user
            SignInManager.SignOut();

            //Redirect to home page
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: UserManagement/User/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });

            return View();
        }

        // POST: UserManagement/User/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
                return View(model);

            //Make sure there are no other accounts associated with the passed in email address
            MI_USER user = DBContext.MI_USER.FirstOrDefault(x => x.EMAIL == model.Email);
            if (user != null)
            {
                ModelState.AddModelError("", "This email address is already in use");
                return View(model);
            }

            //Validate that the password meets the criteria
            if (!MIPassword.CheckPassword(model.Password))
            {
                ModelState.AddModelError("", "The password does not meet the required criteria.");
                return View(model);
            }

            //Create the new user object
            user = new MI_USER { FIRSTNAME = model.FirstName,
                                 LASTNAME = model.LastName,
                                 EMAIL = model.Email};

            //Insert it into the database
            await user.InsertAsync(DBContext, model.Password);

            //Sign in the user. Assume that they do not want the login to persist
            SignInManager.SignIn(user, false);

            //Redirect to the warehouse page
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
        }
        
        // GET: UserManagement/User/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });

            return View();
        }

        // POST: UserManagement/User/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Look up the user by email
            MI_USER user = DBContext.MI_USER.FirstOrDefault(x => x.EMAIL == model.Email);
            if (user == null)
            {
                //The passed in email is not associated with an account
                ModelState.AddModelError("", "Invalid email address");
                return View(model);
            }

            //Since the password isn't actually stored anywhere, we need to update the password to a random string.
            //and then we can send that string to the user's email.
            //After that, they can reset it.
            string newPass = GenerateRandomString();

            //Update the password
            user.UpdatePassword(DBContext, newPass);

            //Send an email to the user with the new password
            //TODO: Email the user

            //Confirm to the user that an email has been sent.
            return RedirectToAction("EmailSent", "User", new { area = "UserManagement" });

        }

        // GET: UserManagement/User/ChangePassword
        public ActionResult ChangePassword()
        {
            //Look up the currently logged in user
            int userID;
            string claimsUserID = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(claimsUserID) || !int.TryParse(claimsUserID, out userID))
            {
                //Did not get a valid value back from the claims. Something is wrong there, so just sign out the user and take them back to the sign in page.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Return the view
            return View(new ChangePasswordViewModel { UserID = userID});
        }

        // POST: UserManagement/User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //Validate the current password
            MI_USER user = DBContext.MI_USER.FirstOrDefault(x => x.USER_ID == model.UserID);
            if (user == null)
            {
                //Failed to look up the user. We should never get here, but put an error into the model state and return the view
                ModelState.AddModelError("", "Failed to load user profile. Please exit this page and try again.");
                return View(model);
            }
            if (!await user.ValidatePasswordAsync(DBContext, model.Password))
            {
                //The current password is invalid
                ModelState.AddModelError("", "The current password is invalid.");
                return View(model);
            }

            //Validate that the new password is not the same as the old password
            if (model.Password == model.NewPassword)
            {
                ModelState.AddModelError("", "The new password is the same as the current password.");
                return View(model);
            }

            //Validate that the new password meets the requirements
            if (!MIPassword.CheckPassword(model.NewPassword))
            {
                ModelState.AddModelError("", "The password does not meet the required criteria.");
                return View(model);
            }

            //Update the database with the new password
            await user.UpdatePasswordAsync(DBContext, model.NewPassword);

            //Return the user back to their user page
            return RedirectToAction("Index", "User", new { area = "UserManagement" });
        }

        // GET: UserManagement/User/UpdateUser
        public ActionResult UpdateUser()
        {
            //Look up the currently logged in user
            int userID;
            string claimsUserID = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(claimsUserID) || !int.TryParse(claimsUserID, out userID))
            {
                //Did not get a valid value back from the claims. Something is wrong there, so just sign out the user and take them back to the sign in page.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the data from the database
            MI_USER user = DBContext.MI_USER.FirstOrDefault(x => x.USER_ID == userID);
            if (user == null)
            {
                //Failed to load the data from the databse. We have to assume that the ID was invalid, so sign out the user and make them sign in again.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Return the view
            return View(new UpdateUserViewModel
            {
                FirstName = user.FIRSTNAME,
                LastName = user.LASTNAME,
                Email = user.EMAIL,
                UserID = user.USER_ID
            });

        }

        // POST: UserManagement/User/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(UpdateUserViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
                return View();

            //Trnasfer the data into a MI_USER object
            MI_USER user = new MI_USER
            {
                FIRSTNAME = model.FirstName,
                LASTNAME = model.LastName,
                EMAIL = model.Email,
                USER_ID = model.UserID
            };

            //Update the user
            await user.UpdateDataAsync(DBContext);

            //Return the user to the default user page
            return RedirectToAction("Index", "User", new { area = "UserManagement" });
        }

        // GET: UserManagement/User/EmailSent
        [AllowAnonymous]
        public ActionResult EmailSent()
        {
            return View();
        }

        // GET: UserManagement/User/DisableUser
        public ActionResult DisableUser()
        {
            //Look up the currently logged in user
            int userID;
            string claimsUserID = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(claimsUserID) || !int.TryParse(claimsUserID, out userID))
            {
                //Did not get a valid value back from the claims. Something is wrong there, so just sign out the user and take them back to the sign in page.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Return the View
            return View(new DisableUserViewModel()
            {
                UserID = userID
            });
        }

        // POST: UserManagement/User/DisableUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableUser(DisableUserViewModel model)
        {
            //Look up the user in the database
            MI_USER user = DBContext.MI_USER.FirstOrDefault(x => x.USER_ID == model.UserID);
            if (user == null)
            {
                //Somehow we failed to load the user. Send them back to the user home page
                return RedirectToAction("Index", "User", new { area = "UserManagmenet" });
            }

            //Find the status number associated with Inactive and set the user's status to that number
            MI_STATUS_REF status = DBContext.MI_STATUS_REF.FirstOrDefault(x => x.STATUS == "Inactive");
            if (status == null)
            {
                //Something went wrong. We weren't able to load the reference. Send them back to the User home page
                return RedirectToAction("Index", "User", new { area = "UserManagmenet" });
            }
            user.STATUS_ID = status.STATUS_ID;

            //Update the user account
            await user.UpdateStatusAsync(DBContext);

            //Sign out the user
            SignInManager.SignOut();

            //Return to home page
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private static string GenerateRandomString()
        {
            //This function will create a random length string of random characters
            Random rnd = new Random();

            //Generate the length that this string will be
            int length = rnd.Next(15, 31);

            string retVal = string.Empty;

            //Loop through and add a random character to the string
            for (int i = 0; i < length; i++)
            {
                retVal += (char)rnd.Next(33, 126);
            }

            //Return the string
            return retVal;
        }
    }
}