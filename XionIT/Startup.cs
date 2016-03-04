using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XionIT.Startup))]
namespace XionIT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
