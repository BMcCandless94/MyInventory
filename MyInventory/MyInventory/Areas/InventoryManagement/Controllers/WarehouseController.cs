using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MyInventory.Areas.InventoryManagement.Warehouse.Models;
using MyInventory.Areas.InventoryManagement.Items.Models;
using MILibrary.Database;
using MILibrary.Database.Entities;
using MILibrary.Database.Extensions;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;

namespace MyInventory.Areas.InventoryManagement.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
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


        #region Warehouse
        // GET: InventoryManagement/Warehouse
        public ActionResult Index(int WarehouseID = -1)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Declare and init the view model
            IndexViewModel model = new IndexViewModel()
            {
                UserID = CurrUser.USER_ID,
                Warehouses = new List<IndexViewModel.IndexWarehouse>()
            };

            //Load in the warehouse info
            //See if there is a specific warehouse we should be looking at.
            if (WarehouseID != -1)
            {
                //Try to load it
                MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active").FirstOrDefault(x => x.WAREHOUSE_ID == WarehouseID);
                if (wh != null)
                {
                    model.Warehouses.Add(new IndexViewModel.IndexWarehouse()
                    {
                        WarehouseID = wh.WAREHOUSE_ID,
                        Name = wh.NAME,
                        Description = wh.DESCRIPTION,
                        Items = new List<IndexViewModel.IndexItem>()
                    });
                }
            }

            //If we did not load a specific warehouse, load in all of the warehouses that belong to this user
            if (model.Warehouses.Count() == 0)
            {
                foreach (MI_WAREHOUSE wh in CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active"))
                {
                    model.Warehouses.Add(new IndexViewModel.IndexWarehouse()
                    {
                        WarehouseID = wh.WAREHOUSE_ID,
                        Name = wh.NAME,
                        Description = wh.DESCRIPTION,
                        Items = new List<IndexViewModel.IndexItem>()
                    });
                }

                //Sort them by name
                model.Warehouses.Sort((x, y) => x.Name.CompareTo(y.Name));
            }

            //Check if there is only one warehouse loaded
            if (model.Warehouses.Count() == 1)
            {
                //Set the specific warehouse flag
                model.WarehouseSpecific = true;
                WarehouseID = model.Warehouses[0].WarehouseID;

                //Load in the items for that warehouse
                MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active").FirstOrDefault(x => x.WAREHOUSE_ID == model.Warehouses[0].WarehouseID);
                if (wh != null)
                {
                    //Load in the items for this warehouse
                    foreach (MI_WH_ITEM item in wh.MI_WH_ITEM.Where(x => x.MI_STATUS_REF.STATUS == "Active"))
                    {
                        model.Warehouses[0].Items.Add(new IndexViewModel.IndexItem()
                        {
                            ItemID = item.ITEM_ID,
                            Name = item.NAME,
                            Description = item.DESCRIPTION,
                            Quantity = item.QUANTITY,
                            UOM = item.UOM,
                            Price = item.PRICE
                        });
                    }

                    //Sort the items
                    model.Warehouses[0].Items.Sort((x, y) => x.Name.CompareTo(y.Name));

                    //Add the transaction types to the ViewData
                    ViewData.Add("TransactionTypes", DBContext.MI_TRANSACTIONTYPE_REF.OrderBy(x => x.TRANSACTIONTYPE).ToList());
                }
            }

            //Load the sidebar data
            LoadSidebarData(WarehouseID);

            //Return the view
            return View(model);
        }

        // GET: InventoryManagement/Warehouse/CreateWarehouse
        public ActionResult CreateWarehouse()
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Return the view
            return View();
        }

        // POST: InventoryManagement/Warehouse/CreateWarehouse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWarehouse(CreateWarehouseViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Check if the warehouse name for the user is already in use
            if (CurrUser.MI_WAREHOUSE.Where(x => x.NAME == model.Name && x.MI_STATUS_REF.STATUS == "Active").Count() > 0)
            {
                //This name is already being used by this user
                ModelState.AddModelError("", "This warehouse name is already in use");
                return View(model);
            }

            //Nothing else to check. Update the database and return to the single view for this warehouse
            MI_WAREHOUSE wh = new MI_WAREHOUSE()
            {
                USER_ID = CurrUser.USER_ID,
                NAME = model.Name,
                DESCRIPTION = model.Description
            };
            int whID = await wh.InsertAsync(DBContext);

            //Redirect to view this warehouse
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement", WarehouseID = whID });
        }

        // GET: InventoryManagement/Warehouse/ManageWarehouse
        public ActionResult ManageWarehouse(int WarehouseID)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the warehouse
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active").FirstOrDefault(x => x.WAREHOUSE_ID == WarehouseID);
            if (wh == null)
            {
                //Either the warehouse doesn't exist or it doesn't belong to this user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load the data into a View Model and return the view
            return View(new ManageWarehouseViewModel()
            {
                WarehouseID = wh.WAREHOUSE_ID,
                Name = wh.NAME,
                Description = wh.DESCRIPTION
            });
        }

        // POST: InventoryManagement/Warehouse/ManageWarehouse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageWarehouse(ManageWarehouseViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the warehouse object
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active").FirstOrDefault(x => x.WAREHOUSE_ID == model.WarehouseID);
            if (wh == null)
            {
                //Either this warehouse doesn't exist or it doesn't belong to this user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Put the new data into the object
            wh.NAME = model.Name;
            wh.DESCRIPTION = model.Description;

            //Make sure there are no other warehouses with this name
            if (CurrUser.MI_WAREHOUSE.Where(x => x.NAME == wh.NAME && x.WAREHOUSE_ID != model.WarehouseID && x.MI_STATUS_REF.STATUS == "Active").Count() > 0)
            {
                //There is already a warehouse with this name
                ModelState.AddModelError("", "This warehouse name is already in use");
                return View(model);
            }

            //Update the database
            await wh.UpdateDataAsync(DBContext);

            //Return to the single view for this warehouse
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement", WarehouseID = wh.WAREHOUSE_ID});
        }

        // GET: InventoryManagement/Warehouse/DisableWarehouse
        public ActionResult DisableWarehouse(int WarehouseID)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Get a reference to the warehouse
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active").FirstOrDefault(x => x.WAREHOUSE_ID == WarehouseID);
            if (wh == null)
            {
                //Either the user does not own this warehouse or it does not exits. Redirect
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Put the data into the view model and return it
            return View(new DisableWarehouseViewModel()
            {
                WarehouseID = wh.WAREHOUSE_ID,
                Name = wh.NAME
            });
        }

        // POST: InventoryManagement/Warehouse/DisableWarehouse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableWarehouse(DisableWarehouseViewModel model)
        {
            //Load the warehouse object
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active").FirstOrDefault(x => x.WAREHOUSE_ID == model.WarehouseID);
            if (wh == null)
            {
                //This shouldn't happen, but it either means that it doesn't exist or doesn't belong to the user.
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load a reference to the inactive status ref
            MI_STATUS_REF status = DBContext.MI_STATUS_REF.FirstOrDefault(x => x.STATUS == "Inactive");
            if (status == null)
            {
                //Failed to load this from the database.
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Set the status on the warehouse object
            wh.STATUS_ID = status.STATUS_ID;

            //Update the database
            await wh.UpdateStatusAsync(DBContext);

            //Return the user to the all warehouse view
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
        }
        #endregion

        #region Item

        // GET: InventoryManagement/Warehouse/CreateItem
        public ActionResult CreateItem(int WarehouseID)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Make sure we can load the warehouse
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.WAREHOUSE_ID == WarehouseID && x.MI_STATUS_REF.STATUS == "Active");
            if (wh == null)
            {
                //Either this warehouse doesn't exist or it doesn't belong to the current user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load the sidebar data
            LoadSidebarData(wh.WAREHOUSE_ID);

            //Fill in the warehouse ID on the view model and return the view
            return View(new CreateItemViewModel()
            {
                WarehouseID = wh.WAREHOUSE_ID
            });
        }

        // POST: InventoryManagement/Warehouse/CreateItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateItem(CreateItemViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
            {
                //Load the sidebar data
                LoadSidebarData(model.WarehouseID);

                return View(model);
            }

            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Make sure we can load the warehouse
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.WAREHOUSE_ID == model.WarehouseID && x.MI_STATUS_REF.STATUS == "Active");
            if (wh == null)
            {
                //Either this warehouse doesn't exist or it doesn't belong to the current user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Check if there is an item with this name already in this warehosue
            if (wh.MI_WH_ITEM.Where(x => x.NAME == model.Name && x.MI_STATUS_REF.STATUS == "Active").Count() > 0)
            {
                //Load the sidebar data
                LoadSidebarData(wh.WAREHOUSE_ID);

                //There is already an item with this name in the warehouse
                ModelState.AddModelError("", "This item name is already in use in this warehouse");
                return View(model);
            }

            //Create a new item and insert it into the database
            MI_WH_ITEM item = new MI_WH_ITEM()
            {
                WAREHOUSE_ID = wh.WAREHOUSE_ID,
                NAME = model.Name,
                DESCRIPTION = model.Description,
                PRICE = model.Price,
                UOM = model.UOM
            };
            await item.InsertAsync(DBContext);

            //Return to the warehouse view
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement", WarehouseID = wh.WAREHOUSE_ID });
        }

        // GET: InventpryManagement/Warehouse/ManageItem
        public ActionResult ManageItem(int ItemID)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the item
            MI_WH_ITEM item = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.MI_WH_ITEM.FirstOrDefault(y => y.ITEM_ID == ItemID) != null)?.MI_WH_ITEM.First(x => x.ITEM_ID == ItemID);
            if (item == null)
            {
                //Either the item doesn't exist or doesn't belong to this user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load the sidebar data
            LoadSidebarData(item.WAREHOUSE_ID);

            //Load the data into the view model and return the view
            return View(new ManageItemViewModel()
            {
                WarehouseID = item.WAREHOUSE_ID,
                ItemID = item.ITEM_ID,
                Name = item.NAME,
                Description = item.DESCRIPTION,
                UOM = item.UOM,
                Price = item.PRICE
            });
        }

        // POST: InventoryManagement/Warehouse/ManageItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageItem(ManageItemViewModel model)
        {
            //Validate the model state
            if (!ModelState.IsValid)
            {
                //Load the sidebar data
                LoadSidebarData(model.WarehouseID);

                return View(model);
            }

            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the warehouse
            MI_WAREHOUSE wh = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.MI_WH_ITEM.FirstOrDefault(y => y.ITEM_ID == model.ItemID) != null);
            if (wh == null)
            {
                //Either the warehouse doesn't exist or belong to this user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load the item
            MI_WH_ITEM item = wh.MI_WH_ITEM.FirstOrDefault(x => x.ITEM_ID == model.ItemID);
            if (item == null)
            {
                //Something went wrong. We should have found the item considering we were able to narrow down the warehouse.
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement", WarehouseID = wh.WAREHOUSE_ID });
            }

            //Make sure there isn't already an item with this name
            if (wh.MI_WH_ITEM.Where(x => x.ITEM_ID != item.ITEM_ID && x.NAME == model.Name).Count() > 0)
            {
                //Load the sidebar data
                LoadSidebarData(wh.WAREHOUSE_ID);

                //This item name already exists
                ModelState.AddModelError("", "This item name is already in use in this warehouse");
                return View(model);
            }

            //Update the item with the data in the view model and update the database
            item.NAME = model.Name;
            item.DESCRIPTION = model.Description;
            item.UOM = model.UOM;
            item.PRICE = model.Price;
            await item.UpdateDataAsync(DBContext);

            //Return to the warehouse view
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement", WarehouseID = wh.WAREHOUSE_ID });
        }

        // GET: InventoryManagement/Warehouse/DisableItem
        public ActionResult DisableItem(int ItemID)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the item
            MI_WH_ITEM item = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.MI_WH_ITEM.FirstOrDefault(y => y.ITEM_ID == ItemID) != null)?.MI_WH_ITEM.First(x => x.ITEM_ID == ItemID);
            if (item == null)
            {
                //Either the item doesn't exist or doesn't belong to this user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load the sidebar data
            LoadSidebarData(item.WAREHOUSE_ID);

            //Load the data into the view model and return the view
            return View(new DisableItemViewModel()
            {
                ItemID = item.ITEM_ID,
                Name = item.NAME
            });
        }

        // POST: InventoryManagement/Warehouse/DisableItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableItem(DisableItemViewModel model)
        {
            //Make sure we can access the currently logged in user's data
            if (CurrUser == null)
            {
                //Could not look up the current user, so sign out and make them sign back in.
                SignInManager.SignOut();
                return RedirectToAction("SignIn", "User", new { area = "UserManagement" });
            }

            //Load the item
            MI_WH_ITEM item = CurrUser.MI_WAREHOUSE.FirstOrDefault(x => x.MI_WH_ITEM.FirstOrDefault(y => y.ITEM_ID == model.ItemID) != null)?.MI_WH_ITEM.First(x => x.ITEM_ID == model.ItemID);
            if (item == null)
            {
                //Either the item doesn't exist or doesn't belong to this user
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Load the reference to the inactive MI_STATUS_REF
            MI_STATUS_REF status = DBContext.MI_STATUS_REF.FirstOrDefault(x => x.STATUS == "Inactive");
            if (status == null)
            {
                //Failed to load this from the database.
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });
            }

            //Set the status on the item object
            item.STATUS_ID = status.STATUS_ID;

            //Update the status in the database
            await item.UpdateStatusAsync(DBContext);

            //Return to the warehouse view
            return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement", WarehouseID = item.WAREHOUSE_ID });
        }

        #endregion

        private void LoadSidebarData(int WarehouseID = -1)
        {
            //Declare and init the view model
            SideBarViewModel model = new SideBarViewModel()
            {
                Warehouses = new List<SideBarViewModel.SideBarWarehouse>()
            };

            //Load any warehouses for the current user and add them to the list in the view model
            foreach (MI_WAREHOUSE wh in CurrUser.MI_WAREHOUSE.Where(x => x.MI_STATUS_REF.STATUS == "Active"))
            {
                model.Warehouses.Add(new SideBarViewModel.SideBarWarehouse()
                {
                    Name = wh.NAME,
                    ID = wh.WAREHOUSE_ID,
                    Active = wh.WAREHOUSE_ID == WarehouseID
                });
            }

            //Sort the list in alphabetical order
            model.Warehouses.Sort((x, y) => x.Name.CompareTo(y.Name));

            //Add this model to the ViewData so that the side bar will automatically render
            ViewData.Add("WHSidebarData", model);
        }
    }
}