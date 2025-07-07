using PurchaseManagement.API.Models;

namespace PurchaseManagement.API.Services.Interfaces
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<Vendor?> GetVendorByIdAsync(int id);
        Task<Vendor> CreateVendorAsync(Vendor vendor);
        Task<Vendor> UpdateVendorAsync(Vendor vendor);
        Task<bool> DeleteVendorAsync(int id);
        Task<bool> VendorExistsAsync(int id);
        Task<IEnumerable<Vendor>> SearchVendorsAsync(string searchTerm);

    }
}
