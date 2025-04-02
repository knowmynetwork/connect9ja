using Datify.API.Contracts;
using Datify.API.Data;
using Datify.Shared.Models;

namespace Datify.API.Services;

public interface IPropertyService : IServices
{
    Task<IEnumerable<PropertyDto>> GetPropertiesAsync();
    Task<PropertyDto?> GetPropertyByIdAsync(int id);
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createItemDto);
    Task<PropertyDto?> UpdatePropertyAsync(int id, PropertyDto propertyDto);
    Task<bool> DeletePropertyAsync(int id);
    public Task<PropertyBooking> SavePropertyBooking(BookingCreationDto booking, CancellationToken cancellationToken);

    Task<IEnumerable<PropertyDto>> GetPropertiesByUserIdAsync(string userId);
    Task CreateOrUpdatePropertyAsync(CreatePropertyDto submission, long Id);
    Task<List<PropertyDto>> SearchPropertiesAsync(SearchPropertyDto propertyDto);

    Task<List<PropertyBookingDto>> GetBookingListByUserIdAsync(string id);

    
    Task<bool> VerifyPropertyPayment(VerifyTransactionDto verifyTransactionDto, CancellationToken cancellationToken);
}