using Jtbuk.ServiceBus.Common;
using Jtbuk.ServiceBus.Data;
using Jtbuk.ServiceBus.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Jtbuk.ServiceBus.Features.Users;

public record CreateUserDto(string Name, Guid TenantUniqueId);

public static class CreateUserAction
{
    public static async Task<ApiValueResponse<Guid>> Invoke([FromBody] CreateUserDto dto, DatabaseContext context)
    {
        var user = context.Users.SingleOrDefault(t => t.Name == dto.Name);

        if (user is not null)
        {
            throw new RecordExistsException<User>(dto.Name);
        }

        var tenant = context.Tenants.SingleOrDefault(t => t.UniqueId == dto.TenantUniqueId);

        if (tenant is null)
        {
            throw new NotFoundException<Tenant>(dto.TenantUniqueId);
        }

        user = new User(dto.Name);

        context.Users.Add(user);

        await context.SaveChangesAsync();

        return new ApiValueResponse<Guid>(user.UniqueId);
    }
}