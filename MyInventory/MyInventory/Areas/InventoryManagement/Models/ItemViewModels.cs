using System.ComponentModel.DataAnnotations;
using MILibrary.Constants;

namespace MyInventory.Areas.InventoryManagement.Items.Models
{
    public class CreateItemViewModel
    {
        public int WarehouseID { get; set; }

        [Required]
        [Display(Name = "Item Name", Description = "The name of the item")]
        [StringLength(Constants.ITM_NAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Name { get; set; }

        [Display(Name = "Description", Description = "A short description of what the item is")]
        [StringLength(Constants.ITM_DESC_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Description { get; set; }

        [Display(Name = "Unit Of Measure", Description = "The unit of measure used to store items in the warehouse")]
        [StringLength(Constants.ITM_UOM_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string UOM { get; set; }

        [Display(Name = "Price", Description = "How much this item costs per UOM")]
        [Range(0.0, double.MaxValue, ErrorMessage = "The Price cannot be below 0")]
        public decimal? Price { get; set; }
    }

    public class ManageItemViewModel
    {
        public int WarehouseID { get; set; }
        public int ItemID { get; set; }

        [Required]
        [Display(Name = "Item Name", Description = "The name of the item")]
        [StringLength(Constants.ITM_NAME_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Name { get; set; }

        [Display(Name = "Description", Description = "A short description of what the item is")]
        [StringLength(Constants.ITM_DESC_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string Description { get; set; }

        [Display(Name = "Unit Of Measure", Description = "The unit of measure used to store items in the warehouse")]
        [StringLength(Constants.ITM_UOM_MAXLENGTH, ErrorMessage = "{0} cannot be more than {1} characters long")]
        public string UOM { get; set; }

        [Display(Name = "Price", Description = "How much this item costs per UOM")]
        [Range(0.0, double.MaxValue, ErrorMessage = "The Price cannot be below 0")]
        public decimal? Price { get; set; }
    }

    public class DisableItemViewModel
    {
        public int WarehouseID { get; set; }
        public int ItemID { get; set; }
        public string Name { get; set; }
    }
}