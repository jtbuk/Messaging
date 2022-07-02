namespace Jtbuk.ServiceBus.Data.Entities
{
    public class Tenant: UniqueEntity
    {
        public string Name { get; set; }

        public Tenant(string name)
        {
            Name = name;
        }
    }
}
