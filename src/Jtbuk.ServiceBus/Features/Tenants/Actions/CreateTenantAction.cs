using Jtbuk.ServiceBus.Common;
using Jtbuk.ServiceBus.Data;
using Jtbuk.ServiceBus.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Jtbuk.ServiceBus.Features.Tenants;

public record CreateTenantDto(string Name);

public static class CreateTenantAction
{
    public static async Task<ApiValueResponse<Guid>> Invoke([FromBody] CreateTenantDto dto, DatabaseContext context)
    {
        var tenant = context.Tenants.SingleOrDefault(t => t.Name == dto.Name);

        if (tenant is not null)
        {
            throw new NotFoundException<Tenant>(dto.Name);
        }

        tenant = new Tenant(dto.Name);

        context.Tenants.Add(tenant);

        await context.SaveChangesAsync();

        return new ApiValueResponse<Guid>(tenant.UniqueId);
    }
}