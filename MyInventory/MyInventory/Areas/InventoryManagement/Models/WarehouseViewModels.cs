using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MILibrary.Constants;

namespace MyInventory.Areas.InventoryManagement.Warehouse.Models
{
    public class SideBarViewModel
    {
        public List<SideBarWarehouse> Warehouses { get; set; }

        public class SideBarWarehouse
        {
            public string Name { get; set; }
            public int ID { get; set; }
            public bool Active { get; set; }
        }
    }

    public class IndexViewModel
    {
        public int UserID { get; set; }
        public bool WarehouseSpecific { get; set; }
        public List<IndexWarehouse> Warehouses { get; set; }


        public class IndexWarehouse
        {
            public int WarehouseID { get; set; }

            [Display(Name = "Warehouse")]
            public string Name { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }

            public List<IndexItem> Items { get; set; }
        }

        public class IndexItem
        {
            public int ItemID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
            public string UOM { get; set; }
            public decimal? Price { get; set; }
        }
    }
    
    public class CreateWarehouseViewModel
    {
        [Required]
        [Display(Name = "Warehouse Name", Description = "The name of the warehouse in which your items will be grouped")]
        [StringLength(Constants.WH_NAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Name { get; set; }

        [Display(Name = "Description", Description = "A description of what the warehouse is used for")]
        [StringLength(Constants.WH_DESC_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Description { get; set; }
    }

    public class ManageWarehouseViewModel
    {
        public int WarehouseID { get; set; }

        [Display(Name = "Warehouse Name", Description = "The name of the warehouse in which your items will be grouped")]
        [Required]
        [StringLength(Constants.WH_NAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Name { get; set; }

        [Display(Name = "Description", Description = "A description of what the warehouse is used for")]
        [StringLength(Constants.WH_DESC_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Description { get; set; }
    }

    public class DisableWarehouseViewModel
    {
        public int WarehouseID { get; set; }
        public string Name { get; set; }
    }
}