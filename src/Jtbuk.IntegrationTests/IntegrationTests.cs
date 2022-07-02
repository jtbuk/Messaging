using Jtbuk.ServiceBus.Common;
using Jtbuk.ServiceBus.Features.Applications;
using Jtbuk.ServiceBus.Features.Applications.Actions;
using Jtbuk.ServiceBus.Features.Entitlements;
using Jtbuk.ServiceBus.Features.Tenants;
using Jtbuk.ServiceBus.Features.Users;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Jtbuk.IntegrationTests
{    
    public class IntegrationTests
    {        
        [Fact]
        public async void CreateEndToEnd()
        {
            var mockLogger = new Mock<ILogger<NotifyEntitlementDto>>();
            var logger = mockLogger.Object;

            var applicationOneConsumer = new GettingStartedConsumer(mockLogger.Object);

            //Maybe do something like this for MassTransit?
            //https://stackoverflow.com/questions/57541541/configure-masstransit-for-testing-with-webapplicationfactorystartup
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
            var client = factory.CreateClient();

            //Register some apps
            var applicationOneRoles = new List<RegisterRoleDto>()
            {
                new("Role A", Guid.NewGuid()),
                new("Role B", Guid.NewGuid()),
            };
            var applicationOne = new RegisterApplicationDto("Jack App", Guid.NewGuid(), applicationOneRoles);

            var applicationTwoRoles = new List<RegisterRoleDto>()
            {
                new("Admin", Guid.NewGuid()),
                new("Sales", Guid.NewGuid()),
            };
            var applicationTwo = new RegisterApplicationDto("Jack App", Guid.NewGuid(), applicationTwoRoles);

            var applicationOneResult = await client.PutAsJsonAsync("api/applications", applicationOne);
            applicationOneResult.EnsureSuccessStatusCode();

            var applicationTwoResult = await client.PutAsJsonAsync("api/applications", applicationTwo);
            applicationTwoResult.EnsureSuccessStatusCode();

            //Add a tenant
            var tenant = new CreateTenantDto("Tenant");
            var createTenantResult = await client.PostAsJsonAsync("api/tenants", tenant);
            createTenantResult.EnsureSuccessStatusCode();
            var tenantUniqueId = (await createTenantResult.Content.ReadFromJsonAsync<ApiValueResponse<Guid>>())!.Value;

            //Add a user
            var user = new CreateUserDto("User", tenantUniqueId);
            var createUserResult = await client.PostAsJsonAsync("api/users", tenant);
            createUserResult.EnsureSuccessStatusCode();
            var userUniqueId = (await createUserResult.Content.ReadFromJsonAsync<ApiValueResponse<Guid>>())!.Value;

            //Get the list of availabile applications
            var applications = await client.GetFromJsonAsync<List<GetApplicationDto>>("api/applications");

            //Use entitlement to grant access for user
            foreach (var application in applications!)
            {
                //application.UniqueId
                var role = application.Roles.First();
                await client.PutAsJsonAsync("api/entitlements", new SetEntitlementDto(userUniqueId, application.UniqueId, role.UniqueId));
            }

            mockLogger.Setup(l => l.LogInformation(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Callback(new InvocationAction(invocation => {
                var args = invocation.Arguments[0];
            }));

            await Task.Delay(10000);
        }
    }

    public class GettingStartedConsumer : IConsumer<NotifyEntitlementDto>
    {
        readonly ILogger<NotifyEntitlementDto> _logger;

        public GettingStartedConsumer(ILogger<NotifyEntitlementDto> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<NotifyEntitlementDto> context)
        {
            _logger.LogInformation(
                "Entitlement Created: {UserUniqueId} {ApplicationUniqueId} {RoleUniqueId}", 
                    context.Message.UserUniqueId, 
                    context.Message.ApplicationUniqueId, 
                    context.Message.RoleUniqueId);

            return Task.CompletedTask;
        }
    }
}