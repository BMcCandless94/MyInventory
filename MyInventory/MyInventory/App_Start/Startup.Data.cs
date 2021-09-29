using System.Configuration;
using Owin;
using MILibrary.Database;

namespace MyInventory
{

    public partial class Startup
    {
        public void ConfigureData(IAppBuilder app)
        {
            //Configure the db context
            app.CreatePerOwinContext<AppDbContext>(() => AppDbContext.Create(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString));
        }
    }
}