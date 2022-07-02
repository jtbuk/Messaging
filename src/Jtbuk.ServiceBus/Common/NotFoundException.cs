namespace Jtbuk.ServiceBus.Common
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public CustomError ErrorObject => new(Message);
    }


    public class NotFoundException<TEntity> : NotFoundException
    {
        public NotFoundException(int id) : base($"{typeof(TEntity).Name} not found with the id {id}") { }
        public NotFoundException(string name) : base($"{typeof(TEntity).Name} not found with the name {name}") { }
        public NotFoundException(Guid uniqueId) : base($"{typeof(TEntity).Name} not found with the Unique Id {uniqueId}") { }
    }
}
