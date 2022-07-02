using Jtbuk.ServiceBus.Common;
using Jtbuk.ServiceBus.Features.Applications;
using Jtbuk.ServiceBus.Features.Applications.Actions;
using Jtbuk.ServiceBus.Features.Entitlements;
using Jtbuk.ServiceBus.Features.Tenants;
using Jtbuk.ServiceBus.Features.Users;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Jtbuk.IntegrationTests
{
    public class IntegrationTests : BaseTest
    {
        [Fact]
        public async void CreateEndToEnd()
        {            
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
            var createUserResult = await client.PostAsJsonAsync("api/users", user);
            createUserResult.EnsureSuccessStatusCode();
            var userUniqueId = (await createUserResult.Content.ReadFromJsonAsync<ApiValueResponse<Guid>>())!.Value;

            //Get the list of availabile applications
            var applications = await client.GetFromJsonAsync<List<GetApplicationDto>>("api/applications");

            var applicationOneHarness = GetTestHarness((o) =>
            {
                o.AddConsumer<NotifyEntitlementConsumer>();
            });
            var applicationTwoHarness = GetTestHarness((o) =>
            {
                o.AddConsumer<NotifyEntitlementConsumer>();
            });

            await applicationOneHarness.Start();
            
            foreach (var application in applications!)
            {
                var role = application.Roles.First();
                await client.PutAsJsonAsync("api/entitlements", new SetEntitlementDto(userUniqueId, application.UniqueId, role.UniqueId));
            }
            
            bool applicationOneConsumed = await applicationOneHarness.Consumed.Any<NotifyEntitlementDto>();
            bool applicationTwoConsumed = await applicationOneHarness.Consumed.Any<NotifyEntitlementDto>();

            Assert.True(applicationOneConsumed);
            Assert.True(applicationTwoConsumed);
        }
    }

    public class NotifyEntitlementConsumer : IConsumer<NotifyEntitlementDto>
    {
        readonly ILogger<NotifyEntitlementDto> _logger;

        public NotifyEntitlementConsumer(ILogger<NotifyEntitlementDto> logger)
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