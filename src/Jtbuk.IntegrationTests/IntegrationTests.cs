using Azure.Messaging.ServiceBus.Administration;
using Jtbuk.ServiceBus.Common;
using Jtbuk.ServiceBus.Features.Applications;
using Jtbuk.ServiceBus.Features.Applications.Actions;
using Jtbuk.ServiceBus.Features.Entitlements;
using Jtbuk.ServiceBus.Features.Tenants;
using Jtbuk.ServiceBus.Features.Users;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
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
            var applicationOne = new RegisterApplicationDto("Generic App", Guid.NewGuid(), new List<RegisterRoleDto>()
            {
                new("Role A", Guid.NewGuid()),
                new("Role B", Guid.NewGuid()),
            });
            
            var applicationTwo = new RegisterApplicationDto("Sales App", Guid.NewGuid(), new List<RegisterRoleDto>()
            {
                new("Admin", Guid.NewGuid()),
                new("Sales", Guid.NewGuid()),
            });
                        
            var applicationThree = new RegisterApplicationDto("Software App", Guid.NewGuid(), new List<RegisterRoleDto>()
            {
                new("Software Engineer", Guid.NewGuid()),
                new("DBA", Guid.NewGuid()),
            });

            var applicationOneResult = await client.PutAsJsonAsync("api/applications", applicationOne);
            applicationOneResult.EnsureSuccessStatusCode();

            var applicationTwoResult = await client.PutAsJsonAsync("api/applications", applicationTwo);
            applicationTwoResult.EnsureSuccessStatusCode();

            var applicationThreeResult = await client.PutAsJsonAsync("api/applications", applicationThree);
            applicationThreeResult.EnsureSuccessStatusCode();

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

            var applicationOneHarness = GetTestHarness(
                cfg => cfg.AddConsumer<NotifyEntitlementConsumer>(),
                azureConfig => azureConfig.SubscriptionEndpoint<NotifyEntitlementConsumer>("applicationOne", c => c.Filter = new CorrelationRuleFilter(applicationOne.UniqueId.ToString()))
            );

            var applicationTwoHarness = GetTestHarness(
                cfg => cfg.AddConsumer<NotifyEntitlementConsumer>(),
                azureConfig => azureConfig.SubscriptionEndpoint<NotifyEntitlementConsumer>("applicationTwo", c => c.Filter = new CorrelationRuleFilter(applicationTwo.UniqueId.ToString()))
            );

            var applicationThreeHarness = GetTestHarness(
                config => config.AddConsumer<NotifyEntitlementConsumer>(),
                azureConfig => azureConfig.SubscriptionEndpoint<NotifyEntitlementConsumer>("applicationThree", c => c.Filter = new CorrelationRuleFilter(applicationThree.UniqueId.ToString()))
            );

            await applicationOneHarness.Start(); 
            await applicationTwoHarness.Start();
            await applicationThreeHarness.Start();

            //Let's add the user to all apps other than application three
            foreach (var application in applications!.Where(a => a.UniqueId != applicationThree.UniqueId))
            {
                var role = application.Roles.First();
                await client.PutAsJsonAsync("api/entitlements", new SetEntitlementDto(userUniqueId, application.UniqueId, role.UniqueId));
            }

            var applicationOneConsumed = applicationOneHarness.Consumed.Count();
            var applicationTwoConsumed = applicationTwoHarness.Consumed.Count();
            var applicationThreeConsumed = applicationThreeHarness.Consumed.Count();            

            Assert.Equal(1, applicationOneConsumed);
            Assert.Equal(1, applicationTwoConsumed);
            Assert.Equal(0, applicationThreeConsumed);            
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