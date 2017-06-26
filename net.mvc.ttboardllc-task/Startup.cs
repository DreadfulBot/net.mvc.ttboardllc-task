using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(net.mvc.ttboardllc_task.Startup))]
namespace net.mvc.ttboardllc_task
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
