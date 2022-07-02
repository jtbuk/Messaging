namespace Jtbuk.ServiceBus.Data.Entities
{
    public class User: UniqueEntity
    {
        public string Name { get; set; }

        public Guid TenantId { get; set; }

        public User(string name)
        {
            Name = name;
        }
    }
}
