using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MILibrary.Database;
using MILibrary.Database.Entities;
using MyInventory.Areas.InventoryManagement.Metrics.Models;

namespace MyInventory.Areas.InventoryManagement.Controllers
{
    [Authorize]
    public class MetricsController : Controller
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

        // GET: InventoryManagement/Metrics
        public ActionResult Index()
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the data into a view model
            MetricsViewModel model = LoadData(CurrUser);

            //Sort the data
            model.Transactions.Sort((x, y) =>
            {
                int result = x.Warehouse.CompareTo(y.Warehouse);
                if (result == 0) result = x.Item.CompareTo(y.Item);
                if (result == 0) result = x.TransactionTime.CompareTo(y.TransactionTime);
                return result;
            });

            //Return the view
            return View(model);
        }

        private MetricsViewModel LoadData(MI_USER user)
        {
            MetricsViewModel model = new MetricsViewModel()
            {
                Transactions = new List<TransactionRecord>()
            };

            //Loop through the warehouses for this user
            foreach (MI_WAREHOUSE wh in user.MI_WAREHOUSE)
            {
                //Loop through the items
                foreach (MI_WH_ITEM item in wh.MI_WH_ITEM)
                {
                    //Loop through any transactions
                    foreach (MI_WH_TRANSACTION tr in item.MI_WH_TRANSACTION)
                    {
                        model.Transactions.Add(new TransactionRecord()
                        {
                            WarehouseID = wh.WAREHOUSE_ID,
                            Warehouse = wh.NAME,
                            ItemID = item.ITEM_ID,
                            Item = item.NAME,
                            UOM = item.UOM,
                            Price = item.PRICE,
                            TransactionID = tr.TRANSACTION_ID,
                            TransactionTypeID = tr.MI_TRANSACTIONTYPE_REF.TRANSACTIONTYPE_ID,
                            TransactionType = tr.MI_TRANSACTIONTYPE_REF.TRANSACTIONTYPE,
                            TransactionAmount = tr.TRANSACTION_AMOUNT,
                            NewAmount = tr.NEW_AMOUNT,
                            TransactionTime = tr.TRANSACTIONDTTM
                        });
                    }
                }
            }

            return model;
        }
    }
}