using Jtbuk.ServiceBus.Features.Applications.Actions;

namespace Jtbuk.ServiceBus.Features.Applications
{
    public static class SetupApplicationsFeature
    {
        public static void AddApplicationsFeature(this WebApplication app)
        {
            //Should be Idemponent because we want to just set the latest values each time an app registers
            app.MapPut("api/applications", RegisterApplicationAction.Invoke);
            app.MapGet("api/applications", GetApplicationsAction.Invoke);
        }
    }
}
