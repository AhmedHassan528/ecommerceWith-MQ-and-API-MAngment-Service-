namespace MultiTenancy.Services.AddressServices
{
    public interface IAddressServices
    {
        Task<AddressModel> GetAddressByID(string userID, int addressID);
        Task<List<AddressModel>> GetUserAddresses(string userID);
        Task<AddressModel> AddAddress(string userID, AddresesesDto address);
        Task<List<AddressModel>> DeleteAddressByID(string userID, int addressID);
    }
}