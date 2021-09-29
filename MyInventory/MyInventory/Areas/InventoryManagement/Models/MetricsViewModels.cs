using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyInventory.Areas.InventoryManagement.Metrics.Models
{
    public class MetricsViewModel
    {
        public List<TransactionRecord> Transactions { get; set; }
    }

    public class TransactionRecord
    {
        public int WarehouseID { get; set; }
        public string Warehouse { get; set; }
        public int ItemID { get; set; }
        public string Item { get; set; }
        public string UOM { get; set; }
        public decimal? Price { get; set; }
        public int TransactionID { get; set; }
        public int TransactionTypeID { get; set; }
        public string TransactionType { get; set; }
        public int TransactionAmount { get; set; }
        public int NewAmount { get; set; }
        public DateTime TransactionTime { get; set; }
    }
}