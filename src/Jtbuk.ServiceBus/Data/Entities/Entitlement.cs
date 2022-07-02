namespace Jtbuk.ServiceBus.Data.Entities
{
    public class Entitlement : UniqueEntity
    {
        public Entitlement(Guid userUniqueId, Guid roleUniqueId, Guid applicationUniqueId)
        {
            UserUniqueId = userUniqueId;
            RoleUniqueId = roleUniqueId;
            ApplicationUniqueId = applicationUniqueId;
        }
        public Guid UserUniqueId { get; set; }
        public Guid RoleUniqueId { get; set; }
        public Guid ApplicationUniqueId { get; set; }
    }
}
