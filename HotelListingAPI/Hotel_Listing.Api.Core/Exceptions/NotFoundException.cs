namespace Hotel_Listing.api.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key) : base($"{name} with id ({key}) was not found")
        {
            
        }
    }
}
