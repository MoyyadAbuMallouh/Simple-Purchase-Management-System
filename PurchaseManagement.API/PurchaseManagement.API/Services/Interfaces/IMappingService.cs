using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Models;
using System.Collections.Generic;

namespace PurchaseManagement.API.Services.Interfaces
{
    public interface IMappingService
    {
        // Product mappings
        ProductDto MapToProductDto(Product product);
        IEnumerable<ProductDto> MapToProductDtos(IEnumerable<Product> products);
        Product MapToProduct(CreateProductDto createProductDto);
        Product MapToProduct(UpdateProductDto updateProductDto, int id);

        // Vendor mappings
        VendorDto MapToVendorDto(Vendor vendor);
        IEnumerable<VendorDto> MapToVendorDtos(IEnumerable<Vendor> vendors);
        Vendor MapToVendor(CreateVendorDto createVendorDto);
        Vendor MapToVendor(UpdateVendorDto updateVendorDto, int id);

        // PurchaseOrder mappings
        PurchaseOrder MapToPurchaseOrder(CreatePurchaseOrderDto dto);
        PurchaseOrder MapToPurchaseOrder(UpdatePurchaseOrderDto dto, int id);
        PurchaseOrderDto MapToPurchaseOrderDto(PurchaseOrder po);
        IEnumerable<PurchaseOrderDto> MapToPurchaseOrderDtos(IEnumerable<PurchaseOrder> poList);

        // User mappings
        UserDto MapToUserDto(Users user);
        IEnumerable<UserDto> MapToUserDtos(IEnumerable<Users> users);
        Users MapToUser(CreateUserDto createUserDto);
        void ApplyUpdate(Users user, UpdateUserDto updateUserDto);

    }
}