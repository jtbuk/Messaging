namespace Jtbuk.ServiceBus.Common
{
    public abstract class RecordExistsException : Exception
    {
        public RecordExistsException(string message) : base(message) { }

        public CustomError ErrorObject => new(Message);
    }


    public class RecordExistsException<TEntity> : RecordExistsException
    {
        public RecordExistsException(int id) : base($"{typeof(TEntity).Name} with the id {id} already exists") { }
        public RecordExistsException(string name) : base($"{typeof(TEntity).Name} with the name {name} already exists") { }
        public RecordExistsException(Guid uniqueId) : base($"{typeof(TEntity).Name} with the Unique Id {uniqueId} already exists") { }
    }
}
