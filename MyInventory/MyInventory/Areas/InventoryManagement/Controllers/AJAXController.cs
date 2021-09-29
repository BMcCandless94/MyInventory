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
using MyInventory.Areas.InventoryManagement.AJAX.Models;

namespace MyInventory.Areas.InventoryManagement.Controllers
{
    public class AJAXController : Controller
    {

        /*
         * NOTE:
         * 
         * Any action that involves a transaction with the database should try to user
         * CurrUser. By using CurrUser we know that any data beeing updated/inserted is 
         * associated with the currently logged in user
         * 
         */

        private AppDbContext _dbContext;
        private AppSignInManager _signInManager;
        private MI_USER _currUser;

        public AppDbContext DBContext
        {
            get { return _dbContext ?? HttpContext.GetOwinContext().Get<AppDbContext>(); }
        }
        public AppSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>(); }
        }
        public MI_USER CurrUser
        {
            get
            {
                if (_currUser == null)
                {
                    //Look up the currently logged in user
                    int userID;
                    string claimsUserID = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    if (string.IsNullOrEmpty(claimsUserID) || !int.TryParse(claimsUserID, out userID))
                    {
                        //Return nothing
                        return null;
                    }

                    //Load the data from the database
                    _currUser = DBContext.MI_USER.FirstOrDefault(x => x.USER_ID == userID);
                }

                return _currUser;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PerformTransaction(PerformTransactionModel model)
        {

            //Make sure that we can load the current user
            if (CurrUser == null)
            {
                return Json(new
                {
                    Success = false,
                    Error = "Failed to load User. Please log out and log back in."
                });
            }

            //Load the item
            MI_WH_ITEM item = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.MI_WH_ITEM.FirstOrDefault(y => y.ITEM_ID == model.ItemID && y.MI_STATUS_REF.STATUS == "Active") != null)?.MI_WH_ITEM.FirstOrDefault(y => y.ITEM_ID == model.ItemID && y.MI_STATUS_REF.STATUS == "Active");
            if (item == null)
            {
                //Failed to load the item
                return Json(new
                {
                    Success = false,
                    Error = "Failed to load Item. Please try again or contact support if the issue persists."
                });
            }

            //Load the transaction type
            MI_TRANSACTIONTYPE_REF ttype = DBContext.MI_TRANSACTIONTYPE_REF.FirstOrDefault(x => x.TRANSACTIONTYPE_ID == model.TransactionType);
            if (ttype == null)
            {
                //Failed to load the transaction type reference
                return Json(new
                {
                    Success = false,
                    Error = "Failed to load Transaction Type. Please try again or contact support if the issue persists."
                });
            }

            //Make sure the transaction is valid
            if (ttype.ACTION == "SET" && model.TransactionAmount < 0)
            {
                //Amount cannot be set to less than 0
                return Json(new
                {
                    Success = false,
                    Error = "Item quantity cannot be set to less than 0."
                });
            } else if (ttype.ACTION == "ADJUST" && (item.QUANTITY + model.TransactionAmount < 0))
            {
                //Amount cannot be set to less than 0
                return Json(new
                {
                    Success = false,
                    Error = "The adjusted Item quantity cannot be less than 0."
                });
            }

            //Try to adjust the quantity
            try
            {
                int newQuantity = await item.PerformTransactionAsync(DBContext, ttype, model.TransactionAmount);
                return Json(new
                {
                    Success = true,
                    Quantity = newQuantity
                });
            } catch (Exception ex)
            {
                //Something bad happened
                return Json(new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }
    }
}