using Jtbuk.ServiceBus.Data;
using Jtbuk.ServiceBus.Data.Entities;

namespace Jtbuk.ServiceBus.Features.Applications;

public record RegisterRoleDto(string Name, Guid UniqueId);
public record RegisterApplicationDto(string Name, Guid UniqueId, List<RegisterRoleDto> Roles);

public static class RegisterApplicationAction
{
    public static async Task Invoke(RegisterApplicationDto dto, DatabaseContext context)
    {
        var application = context.Applications.SingleOrDefault(a => a.UniqueId == dto.UniqueId);

        if (application is null)
        {
            application = new Application(dto.Name)
            {
                UniqueId = dto.UniqueId
            };

            context.Add(application);
        }
        else
        {
            application.Name = dto.Name;

            context.Update(application);
        }

        //What do we need to do for existing roles? do we need a process of marking them as deprecated?
        application.Roles.AddRange(dto.Roles.Select(r => new Role(r.Name)
        {
            UniqueId = r.UniqueId
        }));

        await context.SaveChangesAsync();
    }
}