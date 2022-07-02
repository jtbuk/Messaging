namespace Jtbuk.ServiceBus.Features.Entitlements;


public static class SetupEntitlementFeature
{
    public static void AddEntitlementFeature(this WebApplication app)
    {
        app.MapPut("api/entitlements", SetEntitlementAction.Invoke);
    }
}