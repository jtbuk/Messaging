namespace Jtbuk.ServiceBus.Common
{
    public class ApiValueResponse<TData>
    {
        public ApiValueResponse(TData value)
        {
            Value = value;
        }

        public TData Value { get; set; }        
    }
}
