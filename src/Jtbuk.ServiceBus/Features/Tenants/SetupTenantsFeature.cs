using Jtbuk.ServiceBus.Features.Applications;

namespace Jtbuk.ServiceBus.Features.Tenants;

public static class SetupTenantsFeature
{
    public static void AddTenantsFeature(this WebApplication app)
    {
        //Add a tenant
        app.MapPost("api/tenants", CreateTenantAction.Invoke);
        //Update a tenant
        //Get a tenant
        //Get a list of tenants
        //Delete a tenant
    }
}