using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurchaseManagement.API.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        // Create a new purchase order
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);

        // Get a single purchase order by ID (includes vendor & items)
        Task<PurchaseOrderDto> GetPurchaseOrderAsync(int id);

        // Get all purchase orders
        Task<List<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();

        // Update an existing order by ID
        Task UpdatePurchaseOrderAsync(int id, UpdatePurchaseOrderDto dto);

        // Delete an order by ID
        Task DeletePurchaseOrderAsync(int id);

        // Get raw PurchaseOrder entity (used internally or for advanced mapping)
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
    }
}
