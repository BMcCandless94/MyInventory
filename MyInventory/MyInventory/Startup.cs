using Microsoft.Owin;
using Owin;
using MILibrary.Database;

[assembly: OwinStartupAttribute(typeof(MyInventory.Startup))]
namespace MyInventory
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //IMPORTANT
            //ConfigureData must go before ConfigureAuth. ConfigureAuth relies on the db context being intialized in ConfigureData
            ConfigureData(app);
            ConfigureAuth(app);
        }
    }
}
