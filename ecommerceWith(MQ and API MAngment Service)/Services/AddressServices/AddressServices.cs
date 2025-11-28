
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace MultiTenancy.Services.AddressServices
{
    public class AddressServices : IAddressServices
    {
        private readonly ApplicationDbContext _context;

        public AddressServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AddressModel>> GetUserAddresses(string userID)
        {
            try
            {
                return await _context.Addresses.AsNoTracking().Where(x => x.UserID == userID).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AddressModel> AddAddress(string userID, AddresesesDto address)
        {

            try
            {
                AddressModel model = new()
                {
                    UserID = userID,
                    AddressName = address.AddressName,
                    City = address.City,
                    Address = address.Address,
                    PhoneNumber = address.phoneNumber
                };

                _context.Addresses.Add(model);
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public async Task<AddressModel> GetAddressByID(string userID, int addressID)
        {

            try
            {

                if (addressID == null)
                {
                    throw new Exception("address not fount");
                }

                return await _context.Addresses.FirstOrDefaultAsync(x => x.UserID == userID && x.Id == addressID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AddressModel>> DeleteAddressByID(string userID, int addressID)
        {


            try
            {
                var address = await _context.Addresses.FirstOrDefaultAsync(x => x.UserID == userID && x.Id == addressID);

                if (address == null)
                {
                    throw new Exception("address not fount");
                }
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return await _context.Addresses.AsNoTracking().Where(x => x.UserID == userID).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
