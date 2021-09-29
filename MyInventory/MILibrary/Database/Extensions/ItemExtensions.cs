namespace MILibrary.Database.Extensions
{
    using System.Threading.Tasks;
    using MILibrary.Database.Entities;

    public static class ItemExtensions
    {
        //Extension methods for the MI_ITEM database entity
        //Provides external access to the internal stored procedure methods on the AppDbContext class

        public static int Insert(this MI_WH_ITEM Item, AppDbContext Context) => Context.ItemInsert(Item);
        public static async Task<int> InsertAsync(this MI_WH_ITEM Item, AppDbContext Context) => await Context.ItemInsertAsync(Item);

        public static void UpdateData(this MI_WH_ITEM Item, AppDbContext Context) => Context.ItemUpdateData(Item);
        public static async Task UpdateDataAsync(this MI_WH_ITEM Item, AppDbContext Context) => await Context.ItemUpdateDataAsync(Item);

        public static void UpdateStatus(this MI_WH_ITEM Item, AppDbContext Context) => Context.ItemUpdateStatus(Item);
        public static async Task UpdateStatusAsync(this MI_WH_ITEM Item, AppDbContext Context) => await Context.ItemUpdateStatusAsync(Item);

        public static int PerformTransaction(this MI_WH_ITEM Item, AppDbContext Context, MI_TRANSACTIONTYPE_REF Transaction, int Amount) =>
            Context.ItemTransaction(Item, Transaction, Amount);
        public static async Task<int> PerformTransactionAsync(this MI_WH_ITEM Item, AppDbContext Context, MI_TRANSACTIONTYPE_REF Transaction, int Amount) =>
            await Context.ItemTransactionAsync(Item, Transaction, Amount);
    }
}
