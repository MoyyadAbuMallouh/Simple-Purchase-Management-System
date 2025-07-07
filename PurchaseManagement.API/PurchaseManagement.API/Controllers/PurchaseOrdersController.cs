using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ILogger<PurchaseOrdersController> _logger;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService, ILogger<PurchaseOrdersController> logger)
        {
            _purchaseOrderService = purchaseOrderService;
            _logger = logger;
        }

        // GET: api/purchaseorders
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetAll()
        {
            _logger.LogInformation("Getting all purchase orders");

            try
            {
                var orders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve purchase orders");
                return StatusCode(500, new { message = "Error retrieving purchase orders" });
            }
        }

        // GET: api/purchaseorders/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetById(int id)
        {
            _logger.LogInformation("Getting purchase order with ID {Id}", id);

            try
            {
                var order = await _purchaseOrderService.GetPurchaseOrderAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve purchase order");
                return StatusCode(500, new { message = "Error retrieving purchase order" });
            }
        }

        // POST: api/purchaseorders
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Creating purchase order for vendor ID {VendorId}", dto.VendorId);

            try
            {
                var created = await _purchaseOrderService.CreatePurchaseOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create purchase order");
                return StatusCode(500, new { message = "Error creating purchase order" });
            }
        }

        // PUT: api/purchaseorders/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Updating purchase order ID {Id}", id);

            try
            {
                await _purchaseOrderService.UpdatePurchaseOrderAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update purchase order");
                return StatusCode(500, new { message = "Error updating purchase order" });
            }
        }

        // DELETE: api/purchaseorders/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting purchase order ID {Id}", id);

            try
            {
                await _purchaseOrderService.DeletePurchaseOrderAsync(id);
                return Ok(new { message = "Purchase order deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete purchase order");
                return StatusCode(500, new { message = "Error deleting purchase order" });
            }
        }
    }
}
