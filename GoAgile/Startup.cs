using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GoAgile.Startup))]
namespace GoAgile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {           
            ConfigureAuth(app);

            app.MapSignalR();
        }
    }
}
