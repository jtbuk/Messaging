using Jtbuk.ServiceBus.Common;
using Jtbuk.ServiceBus.Data;
using Jtbuk.ServiceBus.Data.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Jtbuk.ServiceBus.Features.Entitlements;

public record SetEntitlementDto(Guid UserUniqueId, Guid ApplicationUniqueId, Guid RoleUniqueId);
public record NotifyEntitlementDto(Guid UserUniqueId, Guid ApplicationUniqueId, Guid RoleUniqueId);

public static class SetEntitlementAction
{
    public static async Task<Guid> Invoke([FromBody] SetEntitlementDto dto, 
        DatabaseContext context,
        IPublishEndpoint publishEndpoint)
    {        
        if (!context.Users.Any(u => u.UniqueId == dto.UserUniqueId))
        {
            throw new NotFoundException<User>(dto.UserUniqueId);
        }

        var applicationWithRoleExists = 
            context.Applications.Select(a => new {
                ApplicationUniqueID = a.UniqueId,
                RoleUniqueId = a.Roles
                    .Select(r => r.UniqueId)
                    .SingleOrDefault(r => r == dto.RoleUniqueId)
            })
            .Any(a => a.ApplicationUniqueID == dto.ApplicationUniqueId && a.RoleUniqueId == dto.RoleUniqueId);

        if (!applicationWithRoleExists)
        {
            throw new NotFoundException($"Application {dto.ApplicationUniqueId} with role {dto.RoleUniqueId} doesn't exist");
        }

        var entitlement = new Entitlement(dto.UserUniqueId, dto.RoleUniqueId, dto.ApplicationUniqueId);

        context.Entitlements.Add(entitlement);

        await context.SaveChangesAsync();

        var notifyEntitlementDto = new NotifyEntitlementDto(dto.UserUniqueId, dto.RoleUniqueId, dto.ApplicationUniqueId);
        
        await publishEndpoint.Publish(notifyEntitlementDto);

        return entitlement.UniqueId;
    }
}