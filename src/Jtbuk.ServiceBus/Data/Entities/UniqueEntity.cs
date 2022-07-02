namespace Jtbuk.ServiceBus.Data.Entities
{

    public class BaseEntity
    {
        public int Id { get; set; }
    }

    public class UniqueEntity : BaseEntity
    {        
        public Guid UniqueId { get; set; }
    }
}
