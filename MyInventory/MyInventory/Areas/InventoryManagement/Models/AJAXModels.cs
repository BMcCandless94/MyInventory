using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyInventory.Areas.InventoryManagement.AJAX.Models
{
    public class PerformTransactionModel
    {
        public int ItemID { get; set; }
        public int TransactionType { get; set; }
        public int TransactionAmount { get; set; }
    }
}