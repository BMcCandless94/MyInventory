namespace MILibrary.Database.Extensions
{
    using System.Threading.Tasks;
    using MILibrary.Database.Entities;

    public static class WarehouseExtensions
    {
        //Extension methods for the MI_WAREHOUSE database entity
        //Provides external access to the internal stored procedure methods on the AppDbContext class

        public static int Insert(this MI_WAREHOUSE Warehouse, AppDbContext Context) => Context.WarehouseInsert(Warehouse);
        public static async Task<int> InsertAsync(this MI_WAREHOUSE Warehouse, AppDbContext Context) => await Context.WarehouseInsertAsync(Warehouse);

        public static void UpdateData(this MI_WAREHOUSE Warehouse, AppDbContext Context) => Context.WarehouseUpdateData(Warehouse);
        public static async Task UpdateDataAsync(this MI_WAREHOUSE Warehouse, AppDbContext Context) => await Context.WarehouseUpdateDataAsync(Warehouse);

        public static void UpdateStatus(this MI_WAREHOUSE Warehouse, AppDbContext Context) => Context.WarehouseUpdateStatus(Warehouse);
        public static async Task UpdateStatusAsync(this MI_WAREHOUSE Warehouse, AppDbContext Context) => await Context.WarehouseUpdateStatusAsync(Warehouse);
    }
}
