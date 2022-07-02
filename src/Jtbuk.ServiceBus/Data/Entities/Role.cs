namespace Jtbuk.ServiceBus.Data.Entities
{
    public class Role
    {
        public Role(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Guid UniqueId { get; set; }
        public string Name { get; set; }        
    }
}
