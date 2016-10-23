using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoAgile.Startup))]
namespace GoAgile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
