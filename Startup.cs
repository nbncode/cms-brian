using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMS_Brian.Startup))]
namespace CMS_Brian
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
