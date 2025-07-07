using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PurchaseManagement.API.Data;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Services
{
    public class VendorService : IVendorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VendorService> _logger;

        public VendorService(ApplicationDbContext context , ILogger<VendorService> logger) 
        {
             _context = context;
             _logger = logger;

        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            _logger.LogInformation("Service: Getting all vendors");

            try
            {
                var vendors = await _context.Vendors
                    .OrderBy(v => v.Name)
                    .ToListAsync();

                _logger.LogInformation("Service: Successfully retrieved {Count} vendors", vendors.Count);
                return vendors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error retrieving all vendors");
                throw;
            }
        }

        public async Task<Vendor?> GetVendorByIdAsync(int id)
        {
            _logger.LogInformation("Service: Getting vendor with ID: {VendorId}", id);

            try
            {
                var vendor = await _context.Vendors.FindAsync(id);

                if (vendor == null)
                {
                    _logger.LogWarning("Service: Vendor with ID {VendorId} not found", id);
                    
                }
                else
                {
                    _logger.LogInformation("Service: Successfully retrieved vendor {VendorName} (ID: {VendorId})",
                        vendor.Name, id);
                }

                return vendor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error retrieving vendor with ID: {VendorId}", id);
                throw;
            }
        }

        public async Task<Vendor> CreateVendorAsync(Vendor vendor)
        {
            _logger.LogInformation("Service: Creating new vendor: {VendorName}", vendor.Name);

            try
            {
                // Validate vendor name uniqueness
                var existingVendor = await _context.Vendors
                    .FirstOrDefaultAsync(v => v.Name.ToLower() == vendor.Name.ToLower());

                if (existingVendor != null)
                {
                    _logger.LogWarning("Service: Vendor with name {VendorName} already exists", vendor.Name);
                    throw new InvalidOperationException($"Vendor with name '{vendor.Name}' already exists");
                }

                // Validate email uniqueness if provided
                if (!string.IsNullOrWhiteSpace(vendor.Email))
                {
                    var existingEmail = await _context.Vendors
                        .FirstOrDefaultAsync(v => v.Email != null && v.Email.ToLower() == vendor.Email.ToLower());

                    if (existingEmail != null)
                    {
                        _logger.LogWarning("Service: Vendor with email {Email} already exists", vendor.Email);
                        throw new InvalidOperationException($"Vendor with email '{vendor.Email}' already exists");
                    }
                }

                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service: Vendor created successfully with ID: {VendorId}", vendor.Id);
                return vendor;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error creating vendor: {VendorName}", vendor.Name);
                throw;
            }
        }

        public async Task<Vendor> UpdateVendorAsync(Vendor vendor)
        {
            _logger.LogInformation("Service: Updating vendor with ID: {VendorId}", vendor.Id);

            try
            {
                // Check if vendor exists
                var existingVendor = await _context.Vendors.FindAsync(vendor.Id);
                if (existingVendor == null)
                {
                    _logger.LogWarning("Service: Attempted to update non-existent vendor with ID: {VendorId}", vendor.Id);
                    throw new InvalidOperationException($"Vendor with ID {vendor.Id} not found");
                }

                // Validate vendor name uniqueness (excluding current vendor)
                var duplicateVendor = await _context.Vendors
                    .FirstOrDefaultAsync(v => v.Name.ToLower() == vendor.Name.ToLower() && v.Id != vendor.Id);

                if (duplicateVendor != null)
                {
                    _logger.LogWarning("Service: Vendor name {VendorName} already exists for another vendor", vendor.Name);
                    throw new InvalidOperationException($"Vendor with name '{vendor.Name}' already exists");
                }

                // Validate email uniqueness if provided (excluding current vendor)
                if (!string.IsNullOrWhiteSpace(vendor.Email))
                {
                    var duplicateEmail = await _context.Vendors
                        .FirstOrDefaultAsync(v => v.Email != null &&
                                                 v.Email.ToLower() == vendor.Email.ToLower() &&
                                                 v.Id != vendor.Id);

                    if (duplicateEmail != null)
                    {
                        _logger.LogWarning("Service: Vendor email {Email} already exists for another vendor", vendor.Email);
                        throw new InvalidOperationException($"Vendor with email '{vendor.Email}' already exists");
                    }
                }

                // Update properties
                existingVendor.Name = vendor.Name;
                existingVendor.Address = vendor.Address;
                existingVendor.ContactPerson = vendor.ContactPerson;
                existingVendor.Phone = vendor.Phone;
                existingVendor.Email = vendor.Email;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Service: Vendor updated successfully. ID: {VendorId}", vendor.Id);
                return existingVendor;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error updating vendor with ID: {VendorId}", vendor.Id);
                throw;
            }
        }

        public async Task<bool> DeleteVendorAsync(int id)
        {
            _logger.LogInformation("Service: Deleting vendor with ID: {VendorId}", id);

            try
            {
                var vendor = await _context.Vendors.FindAsync(id);

                if (vendor == null)
                {
                    _logger.LogWarning("Service: Attempted to delete non-existent vendor with ID: {VendorId}", id);
                    return false;
                }

                // Check if vendor is used in any purchase orders
                var isUsedInOrders = await _context.PurchaseOrders
                    .AnyAsync(po => po.VendorId == id);

                if (isUsedInOrders)
                {
                    _logger.LogWarning("Service: Cannot delete vendor {VendorId} - it's used in purchase orders", id);
                    throw new InvalidOperationException("Cannot delete vendor that is used in purchase orders");
                }

                _context.Vendors.Remove(vendor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service: Vendor deleted successfully. ID: {VendorId}, Name: {VendorName}",
                    id, vendor.Name);

                return true;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error deleting vendor with ID: {VendorId}", id);
                throw;
            }
        }

        public async Task<bool> VendorExistsAsync(int id)
        {
            _logger.LogInformation("Service: Checking if vendor exists with ID: {VendorId}", id);

            try
            {
                var exists = await _context.Vendors.AnyAsync(v => v.Id == id);
                _logger.LogInformation("Service: Vendor {VendorId} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error checking vendor existence with ID: {VendorId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Vendor>> SearchVendorsAsync(string searchTerm)
        {
            _logger.LogInformation("Service: Searching vendors with term: {SearchTerm}", searchTerm);

            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllVendorsAsync();
                }

                var vendors = await _context.Vendors
                    .Where(v => v.Name.ToLower().Contains(searchTerm.ToLower()) ||
                          (v.Address!=null && v.Address.ToLower().Contains(searchTerm.ToLower())) ||
                          (v.ContactPerson != null && v.ContactPerson.ToLower().Contains(searchTerm.ToLower())) ||
                          (v.Email != null && v.Email.ToLower().Contains(searchTerm.ToLower())) ||
                          (v.Phone != null && v.Phone.Contains(searchTerm)))
                    .OrderBy(v => v.Name)
                    .ToListAsync();

                _logger.LogInformation("Service: Found {Count} vendors matching search term: {SearchTerm}",
                    vendors.Count, searchTerm);

                return vendors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error searching vendors with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

    }
}
