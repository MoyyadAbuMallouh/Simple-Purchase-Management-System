using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<VendorsController> _logger;

        public VendorsController(IVendorService vendorService, ILogger<VendorsController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        // GET: api/vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            _logger.LogInformation("Controller: Getting all vendors");

            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                _logger.LogInformation("Controller: Successfully retrieved vendors");
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error retrieving vendors");
                return BadRequest(new { message = "Error retrieving vendors" });
            }
        }

        // GET: api/vendors/search?term=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Vendor>>> SearchVendors([FromQuery] string term = "")
        {
            _logger.LogInformation("Controller: Searching vendors with term: {SearchTerm}", term);

            try
            {
                var vendors = await _vendorService.SearchVendorsAsync(term);
                _logger.LogInformation("Controller: Successfully retrieved search results");
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error searching vendors");
                return BadRequest(new { message = "Error searching vendors" });
            }
        }

        // GET: api/vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            _logger.LogInformation("Controller: Getting vendor with ID: {VendorId}", id);

            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);

                if (vendor == null)
                {
                    _logger.LogWarning("Controller: Vendor with ID {VendorId} not found", id);
                    return NotFound(new { message = $"Vendor with ID {id} not found" });
                }

                _logger.LogInformation("Controller: Successfully retrieved vendor {VendorId}", id);
                return Ok(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error retrieving vendor with ID: {VendorId}", id);
                return BadRequest(new { message = "Error retrieving vendor" });
            }
        }

        // POST: api/vendors
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Vendor>> CreateVendor(Vendor vendor)
        {
            _logger.LogInformation("Controller: Creating new vendor: {VendorName}", vendor.Name);

            // Basic validation
            if (string.IsNullOrWhiteSpace(vendor.Name))
            {
                return BadRequest(new { message = "Vendor name is required" });
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(vendor.Email))
            {
                if (!IsValidEmail(vendor.Email))
                {
                    return BadRequest(new { message = "Invalid email format" });
                }
            }

            try
            {
                var createdVendor = await _vendorService.CreateVendorAsync(vendor);

                _logger.LogInformation("Controller: Vendor created successfully with ID: {VendorId}", createdVendor.Id);

                return CreatedAtAction(nameof(GetVendor), new { id = createdVendor.Id }, createdVendor);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Controller: Validation error creating vendor: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error creating vendor: {VendorName}", vendor.Name);
                return BadRequest(new { message = "Error creating vendor" });
            }
        }

        // PUT: api/vendors/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Vendor>> UpdateVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id)
            {
                _logger.LogWarning("Controller: Vendor ID mismatch. URL ID: {UrlId}, Body ID: {BodyId}", id, vendor.Id);
                return BadRequest(new { message = "ID mismatch" });
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(vendor.Name))
            {
                return BadRequest(new { message = "Vendor name is required" });
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(vendor.Email))
            {
                if (!IsValidEmail(vendor.Email))
                {
                    return BadRequest(new { message = "Invalid email format" });
                }
            }

            try
            {
                var updatedVendor = await _vendorService.UpdateVendorAsync(vendor);

                _logger.LogInformation("Controller: Vendor updated successfully. ID: {VendorId}", id);
                return Ok(updatedVendor);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Controller: Validation error updating vendor: {Message}", ex.Message);

                // Check if it's a not found error
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error updating vendor. ID: {VendorId}", id);
                return BadRequest(new { message = "Error updating vendor" });
            }
        }

        // DELETE: api/vendors/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVendor(int id)
        {
            _logger.LogInformation("Controller: Deleting vendor with ID: {VendorId}", id);

            try
            {
                var result = await _vendorService.DeleteVendorAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Controller: Vendor with ID {VendorId} not found for deletion", id);
                    return NotFound(new { message = $"Vendor with ID {id} not found" });
                }

                _logger.LogInformation("Controller: Vendor deleted successfully. ID: {VendorId}", id);
                return Ok(new { message = "Vendor deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Controller: Cannot delete vendor: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error deleting vendor. ID: {VendorId}", id);
                return BadRequest(new { message = "Error deleting vendor" });
            }
        }

        // GET: api/vendors/exists/5
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> VendorExists(int id)
        {
            _logger.LogInformation("Controller: Checking if vendor exists with ID: {VendorId}", id);

            try
            {
                var exists = await _vendorService.VendorExistsAsync(id);
                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error checking vendor existence. ID: {VendorId}", id);
                return BadRequest(new { message = "Error checking vendor existence" });
            }
        }

        // Helper method for email validation
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Test endpoint for vendor-specific error handling
        [HttpGet("test-service-error")]
        public async Task<ActionResult> TestServiceError()
        {
            _logger.LogInformation("Controller: Testing vendor service error handling");

            try
            {
                // This will cause a service error
                await _vendorService.GetVendorByIdAsync(-1);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Caught service error in test endpoint");
                return BadRequest(new { message = "Service error occurred", details = ex.Message });
            }
        }
    }
}