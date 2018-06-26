using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(OktaCustomerUI.Startup))]
namespace OktaCustomerUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
