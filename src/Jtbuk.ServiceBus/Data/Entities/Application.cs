namespace Jtbuk.ServiceBus.Data.Entities
{
    public class Application
    {
        public Application(string name)
        {
            Name = name;
        }

        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }
        public List<Role> Roles { get; init; } = new List<Role>();
    }
}
