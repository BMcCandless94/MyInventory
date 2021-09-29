namespace MILibrary.Database.Extensions
{
    using System.Threading.Tasks;
    using MILibrary.Database.Entities;

    public static class UserExtentions
    {
        //Extension methods for the MI_USER database entity
        //Provides external access to the internal stored procedure methods on the AppDbContext class

        public static int Insert(this MI_USER User, AppDbContext Context, string Password) => Context.UserInsert(User, Password);
        public static async Task<int> InsertAsync(this MI_USER User, AppDbContext Context, string Password) =>  await Context.UserInsertAsync(User, Password);

        public static void UpdateData(this MI_USER User, AppDbContext Context) => Context.UserUpdateData(User);
        public static async Task UpdateDataAsync(this MI_USER User, AppDbContext Context) => await Context.UserUpdateDataAsync(User);

        public static void UpdatePassword(this MI_USER User, AppDbContext Context, string Password) => Context.UserUpdatePassword(User, Password);
        public static async Task UpdatePasswordAsync(this MI_USER User, AppDbContext Context, string Password) => await Context.UserUpdatePasswordAsync(User, Password);

        public static void UpdateStatus(this MI_USER User, AppDbContext Context) => Context.UserUpdateStatus(User);
        public static async Task UpdateStatusAsync(this MI_USER User, AppDbContext Context) => await Context.UserUpdateStatusAsync(User);

        public static bool ValidatePassword(this MI_USER User, AppDbContext Context, string Password) => Context.UserValidatePassword(User, Password);
        public static async Task<bool> ValidatePasswordAsync(this MI_USER User, AppDbContext Context, string Password) => await Context.UserValidatePasswordAsync(User, Password);
    }
}
