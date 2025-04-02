using System.Linq.Expressions;
using System.Net.Http.Headers;
using AutoMapper;
using Datify.API.Contracts;
using Datify.API.Data;
using Microsoft.EntityFrameworkCore;
using Datify.Shared.Models;
using Datify.Shared.Models.Enum;
using Datify.Shared.Utilities;

namespace Datify.API.Services;

public class PropertyService(
    ApplicationDbContext context,
    IMapper mapper,
    IDocumentService documentService,
    IUserService userService,
    HttpClient httpClient,
    ILoggerFactory loggerFactory)
    : IPropertyService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<PropertyService>();

     public async Task<IEnumerable<PropertyDto>> GetPropertiesAsync()
    {
        var items = await context.Properties
            .AsNoTracking()
            .Include(x => x.PropertyItemDocuments)
            .Include(x => x.User)
            .Select(MapToItemDto())
            .ToListAsync();
        return items;
        //return mapper.Map<IEnumerable<PropertyDto>>(items);
        //return items.Select(MapToItemDto).ToList();
    }

    public async Task<IEnumerable<PropertyDto>> GetPropertiesByUserIdAsync(string userId)
    {
        var items = await context.Properties
            .Include(t => t.PropertyItemDocuments)
            .Include(t => t.User)
            .Where(i => i.UserId == userId)
            .Select(MapToItemDto())
            .ToListAsync();
        return items;

        //return mapper.Map<IEnumerable<ItemDto>>(items);
    }
    
    public async Task<List<PropertyBookingDto>> GetBookingListByUserIdAsync(string userId)
    {
        List<PropertyBookingDto> propertyBookingDtos = new List<PropertyBookingDto>();
        var items = await context.PropertyBookings
            .Where(i => i.BookingUserId == userId)
            .ToListAsync();

        foreach (var booking in items)
        {
            PropertyBookingDto propertyBookingDto = new PropertyBookingDto();
            propertyBookingDto.BookingUserId = booking.BookingUserId;
            propertyBookingDto.PropertyId = booking.PropertyId;
            propertyBookingDto.FirstName = booking.FirstName;
            propertyBookingDto.LastName = booking.LastName;
            propertyBookingDto.Email = booking.Email;
            propertyBookingDto.Price = booking.Price;
            propertyBookingDto.DatePaid = booking.DatePaid;
            propertyBookingDto.IsPaid = booking.IsPaid;
            propertyBookingDto.MobileNo = booking.MobileNo;
            propertyBookingDto.OccupantsNumber = booking.OccupantsNumber;
            propertyBookingDto.AvailableEndDate = booking.AvailableEndDate;
            propertyBookingDto.AvailableStartDate = booking.AvailableStartDate;
            propertyBookingDtos.Add(propertyBookingDto);
        }
        return propertyBookingDtos;
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(int id)
    {
        var item = await context.Properties.FindAsync(id);
        return item == null ? null : mapper.Map<PropertyDto>(item);
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createItemDto)
    {
        throw new NotImplementedException();
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(int id, PropertyDto propertyDto)
    {
        var item = await context.Properties.FindAsync(id);
        if (item == null) return null;

        mapper.Map(propertyDto, item);
        context.Properties.Update(item);
        await context.SaveChangesAsync();
        return mapper.Map<PropertyDto>(item);
    }

    public async Task<bool> DeletePropertyAsync(int id)
    {
        var item = await context.Properties.FindAsync(id);
        if (item == null) return false;

        context.Properties.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task CreateOrUpdatePropertyAsync(CreatePropertyDto propertyDto, long id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
        var propertyEntity = await context.Properties.FindAsync(id);
        if (propertyEntity == null)
        {
            // Create new item
            propertyDto.User = null;
            propertyEntity = mapper.Map<Data.Property>(propertyDto);
            context.Properties.Add(propertyEntity);
            await context.SaveChangesAsync();

        }
        else
        {
            // Update only modified fields
            UpdateOnlyModifiedFields(propertyDto, propertyEntity);

            context.Entry(propertyEntity).State = EntityState.Modified;
        }

            // Handle document updates
            await HandleDocuments(propertyDto, propertyEntity);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
    }

    private static void UpdateOnlyModifiedFields(CreatePropertyDto propertyDto, Property propertyEntity)
    {
        if (propertyEntity.Title != propertyDto.Title) propertyEntity.Title = propertyDto.Title;
        if (propertyEntity.Category != propertyDto.Category) propertyEntity.Category = propertyDto.Category;
        if (propertyEntity.Condition != propertyDto.Condition) propertyEntity.Condition = propertyDto.Condition;
        if (propertyEntity.Location != propertyDto.Location) propertyEntity.Location = propertyDto.Location;
        if (propertyEntity.Town != propertyDto.Town) propertyEntity.Town = propertyDto.Town;
        if (propertyEntity.State != propertyDto.State) propertyEntity.State = propertyDto.State;
        if (propertyEntity.CollectionOption != propertyDto.CollectionOption) propertyEntity.CollectionOption = propertyDto.CollectionOption;
        if (propertyEntity.Description != propertyDto.Description) propertyEntity.Description = propertyDto.Description;
        if (propertyEntity.PaymentFrequency != propertyDto.PaymentFrequency) propertyEntity.PaymentFrequency = propertyDto.PaymentFrequency;
        if (propertyEntity.Price != propertyDto.Price) propertyEntity.Price = propertyDto.Price;
        if (propertyEntity.Currency != propertyDto.Currency) propertyEntity.Currency = propertyDto.Currency;
        if (propertyEntity.Bathrooms != propertyDto.Bathrooms) propertyEntity.Bathrooms = propertyDto.Bathrooms;
        if (propertyEntity.Bedrooms != propertyDto.Bedrooms) propertyEntity.Bedrooms = propertyDto.Bedrooms;
        if (propertyEntity.LivingRooms != propertyDto.LivingRooms) propertyEntity.LivingRooms = propertyDto.LivingRooms;
        if (propertyEntity.PropertyMeasurement != propertyDto.PropertyMeasurement) propertyEntity.PropertyMeasurement = propertyDto.PropertyMeasurement;
        if (propertyEntity.MaxGuests != propertyDto.MaxGuests) propertyEntity.MaxGuests = propertyDto.MaxGuests;
        if (propertyEntity.LandMark != propertyDto.LandMark) propertyEntity.LandMark = propertyDto.LandMark;
        if (propertyEntity.PropertyType != propertyDto.PropertyType) propertyEntity.PropertyType = propertyDto.PropertyType;
        if (propertyEntity.MarketType != propertyDto.MarketType) propertyEntity.MarketType = propertyDto.MarketType;
        // if (propertyEntity.HouseRules != propertyDto.HouseRules) propertyEntity.HouseRules = propertyDto.HouseRules;
        // if (propertyEntity.EventsAllowed != propertyDto.EventsAllowed) propertyEntity.EventsAllowed = propertyDto.EventsAllowed;
        // if (propertyEntity.FinalPricing != propertyDto.FinalPricing) propertyEntity.FinalPricing = propertyDto.FinalPricing;
        // if (propertyEntity.SecurityDeposit != propertyDto.SecurityDeposit) propertyEntity.SecurityDeposit = propertyDto.SecurityDeposit;
        // if (propertyEntity.NightlyRate != propertyDto.NightlyRate) propertyEntity.NightlyRate = propertyDto.NightlyRate;
        // if (propertyEntity.HasParking != propertyDto.HasParking) propertyEntity.HasParking = propertyDto.HasParking;
        // if (propertyEntity.HasFreeWifi != propertyDto.HasFreeWifi) propertyEntity.HasFreeWifi = propertyDto.HasFreeWifi;
        // if (propertyEntity.HasSwimmingPool != propertyDto.HasSwimmingPool) propertyEntity.HasSwimmingPool = propertyDto.HasSwimmingPool;
        // if (propertyEntity.HasAirConditioning != propertyDto.HasAirConditioning) propertyEntity.HasAirConditioning = propertyDto.HasAirConditioning;
        // if (propertyEntity.CheckInTime != propertyDto.CheckInTime) propertyEntity.CheckInTime = propertyDto.CheckInTime;
        // if (propertyEntity.CheckOutTime != propertyDto.CheckOutTime) propertyEntity.CheckOutTime = propertyDto.CheckOutTime;
        // if (propertyEntity.SecurityDepositCondition != propertyDto.SecurityDepositCondition) propertyEntity.SecurityDepositCondition = propertyDto.SecurityDepositCondition;
        // if (propertyEntity.BuildingName != propertyDto.BuildingName) propertyEntity.BuildingName = propertyDto.BuildingName;
        // if (propertyEntity.BuildingNumber != propertyDto.BuildingNumber) propertyEntity.BuildingNumber = propertyDto.BuildingNumber;
        // if (propertyEntity.HouseNumber != propertyDto.HouseNumber) propertyEntity.HouseNumber = propertyDto.HouseNumber;
        
        // Update Features
        UpdateCollection(propertyEntity.Features, propertyDto.Features, (f, dto) => f.FeatureName == dto.FeatureName, dto => new PropertyFeature { FeatureName = dto.FeatureName });

        // Update Property Rules
        UpdateCollection(propertyEntity.PropertyRules, propertyDto.PropertyRules, (r, dto) => r.RuleDescription == dto.RuleDescription, dto => new PropertyRules { RuleDescription = dto.RuleDescription });

        // Update Property Allowed Events
        UpdateCollection(propertyEntity.PropertyAllowedEvents, propertyDto.PropertyAllowedEvents, (e, dto) => e.EventDescription == dto.EventDescription, dto => new PropertyAllowedEvents { EventDescription = dto.EventDescription });

        // Update Ratings
        //UpdateCollection(propertyEntity.Ratings, propertyDto.Ratings, (r, dto) => r.Rating == dto.Rating && r.Review == dto.Review, dto => new PropertyRatings() { Rating = dto.Rating, Review = dto.Review });

    }
    
    // Helper method to update collections efficiently
    private static void UpdateCollection<T, TDto>(
        ICollection<T> existingCollection,
        List<TDto> newCollection,
        Func<T, TDto, bool> matchFunc,
        Func<TDto, T> createNewItemFunc)
    {
        // Find items to remove
        var toRemove = existingCollection.Where(existing => !newCollection.Any(newItem => matchFunc(existing, newItem))).ToList();
        foreach (var item in toRemove)
        {
            existingCollection.Remove(item);
        }

        // Add new items
        var toAdd = newCollection.Where(newItem => !existingCollection.Any(existing => matchFunc(existing, newItem))).ToList();
        foreach (var item in toAdd)
        {
            existingCollection.Add(createNewItemFunc(item));
        }
    }

    public async Task<List<PropertyDto>> SearchPropertiesAsync(SearchPropertyDto propertyDto)
    {
        var propertiesQueryable = context.Properties
            .Include(t => t.PropertyItemDocuments)
            .Include(t => t.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(propertyDto.Location))
        {
            var searchTerm = propertyDto.Location.ToLower();
            propertiesQueryable = propertiesQueryable
                .Where(x => x.Location.ToLower().Contains(searchTerm));
        }

        if (propertyDto.NoOfRooms > 0)
        {
            propertiesQueryable = propertiesQueryable
                .Where(x => (x.Bedrooms + x.Bathrooms + x.LivingRooms) == propertyDto.NoOfRooms);
        }

        if (propertyDto.AvailableStartDate != null && propertyDto.AvailableEndDate != null)
        {
            propertiesQueryable = propertiesQueryable.Where(p =>
                !context.PropertyBookings.Any(b =>
                    b.PropertyId == p.Id &&
                    b.AvailableStartDate <= propertyDto.AvailableEndDate &&
                    b.AvailableEndDate >= propertyDto.AvailableStartDate)
            );
        }

        // Apply projection to only fetch necessary data
        return await propertiesQueryable
            .Select(MapToItemDto())
            .ToListAsync();
    }

    
    public async Task<PropertyBooking> SavePropertyBooking(BookingCreationDto booking, CancellationToken cancellationToken)
    {
        try
        {
            if (!booking.IsBookingForSoneoneElse)
            {
                var user = await userService.GetById(booking.BookingUserId, cancellationToken);
                if (user != null)
                {
                    booking.FirstName = user.FirstName;
                    booking.LastName = user.LastName;
                    booking.Email = user.Email ?? string.Empty;
                }
                else
                {
                    throw new Exception("User not found.");
                }
            }
            var bookingEntity = mapper.Map<PropertyBooking>(booking);
            context.PropertyBookings.Add(bookingEntity);
            await context.SaveChangesAsync();
            return bookingEntity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private async Task HandleDocuments(CreatePropertyDto propertyDto, Property propertyEntity)
    {
        var existingDocuments = await context.PropertyDocuments
            .Where(d => d.ItemId == propertyEntity.Id)
            .ToListAsync();

        List<(bool isSuccess, string message, PropertyDocumentDto item)> upd = [];

        if (propertyDto.DocumentsToUpload.Count > 0)
        {
            foreach (var doc in propertyDto.DocumentsToUpload)
            {
                var existingDoc = existingDocuments.FirstOrDefault(d => d.DocumentName == doc.FileName);
            
                if (existingDoc != null)
                {
                    // Replace document
                    var result = await documentService.ReplacePropertyDocumentAsync(existingDoc.Id, doc, DocumentType.MarketItem.ToString());
                    upd.Add(result);
                }
                else
                {
                    // Upload new document
                    var result = await documentService.UploadPropertyDocumentAsync(propertyEntity.Id, doc, DocumentType.MarketItem.ToString());
                    upd.Add(result);
                }
            }

            if (upd.Any(x => !x.isSuccess)) throw new Exception("Document upload failed.");
        }

        // Delete orphaned documents (after confirming they are not referenced elsewhere)
        foreach (var existing in existingDocuments)
        {
            if (propertyDto.PropertyDocuments.All(x => x.Id != existing.Id))
            {
                var stillExists = await context.PropertyDocuments.AnyAsync(d => d.Id == existing.Id);
                if (!stillExists)
                    await documentService.DeleteItemDocumentAsync(existing.Id);
            }
        }
    }
    

    private static Expression<Func<Property, PropertyDto>> MapToItemDto()
    {
        return p => new PropertyDto
        {
            Id = p.Id,
            Condition = p.Condition,
            CollectionOption = p.CollectionOption,
            Category = p.Category,
            Description = p.Description,
            Location = p.Location,
            Title = p.Title,
            Price = p.Price,
            WhenWasActionDone = p.WhenWasActionDone,
            WhoDidTheAction = p.WhoDidTheAction,
            WhichCommunityWasActionDoneFor = p.WhichCommunityWasActionDoneFor,
            DateInserted = p.DateInserted,
            DateModified = p.DateModified,
            WasActionDoneBySomeoneElse = p.WasActionDoneBySomeoneElse,
            IsDeleted = p.IsDeleted,
            PaymentFrequency = p.PaymentFrequency,
            PropertyMeasurement = p.PropertyMeasurement,
            MaxGuests = p.MaxGuests,
            LandMark = p.LandMark,
            LivingRooms = p.LivingRooms,
            Bedrooms = p.Bedrooms,
            Bathrooms = p.Bathrooms,
            Currency = p.Currency,
            MarketType = p.MarketType,
            State = p.State,
            Town = p.Town,
            PropertyType = p.PropertyType,
            ProximityPlaces = p.ProximityPlaces.Select(pp=> new PropertyProximityPlaceDto{
                Id = pp.Id,
                Distance = pp.Distance,
            }).ToList(),
            PropertyRules = p.PropertyRules.Select(rules => new PropertyRulesDto{
                Id = rules.Id,
                RuleDescription = rules.RuleDescription,
            }).ToList(),
            PropertyAllowedEvents = p.PropertyAllowedEvents.Select(x => new PropertyAllowedEventsDto{
                Id = x.Id,
                EventDescription = x.EventDescription,
            }).ToList(),
            Ratings = p.Ratings.Select(r=> new PropertyRatingDto{
                Id = r.Id,
                Rating = r.Rating,
                Review = r.Review,
            }).ToList(),

            Features = p.Features.Select(f=> new PropertyFeatureDto{
                Id = f.Id,
                FeatureName = f.FeatureName,
            }).ToList(),

            PropertyDocuments = p.PropertyItemDocuments.Select(d => new PropertyDocumentDto
            {
                Id = d.Id,
                DocumentPath = d.DocumentPath,
                DocumentName = d.DocumentName,
                DocumentType = d.DocumentType,
                DateModified = d.DateModified,
                DateInserted = d.DateInserted,
                IsDeleted = d.IsDeleted,
                WhenWasActionDone = d.WhenWasActionDone,
                WhoDidTheAction = d.WhoDidTheAction,
                WhichCommunityWasActionDoneFor = d.WhichCommunityWasActionDoneFor,
                WasActionDoneBySomeoneElse = d.WasActionDoneBySomeoneElse,
                UploadedDate = d.UploadedDate,
                ItemId = d.ItemId
            }).ToList(),

            User = new UserDto
            {
                Id = p.User.Id,
                Name = p.User.FirstName + " " + p.User.LastName,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                PhoneNumber = p.User.PhoneNumber ?? string.Empty,
                Email = p.User.Email ?? string.Empty,
                UserName = p.User.UserName ?? string.Empty
            }
        };
    }
    
    public async Task<bool> VerifyPropertyPayment(VerifyTransactionDto verifyTransactionDto, CancellationToken cancellationToken)
    {
        _logger.LogTrace(
            $"start verifying {verifyTransactionDto.TransactionId} " +
            $"{verifyTransactionDto.Amount} {verifyTransactionDto.Currency}" +
            $"{verifyTransactionDto.RelatedEntityId} " +
            $"{verifyTransactionDto.RelatedEntity} " +
            $"{verifyTransactionDto.UserId}," +
            $" trxref of {verifyTransactionDto.TrxRef}");

        if (await DoesTrnxRefExists(verifyTransactionDto.TrxRef))
        {
            _logger.LogTrace(
                $"already exist so not verifying {verifyTransactionDto.TransactionId}") ;
            return true;
        }

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://api.flutterwave.com/v3/transactions/{verifyTransactionDto.TransactionId}/verify");
        var flutterwaveKey = GetFlutterwaveKey();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", flutterwaveKey);

        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            try
            {
                var readAsStringAsync = await response.Content.ReadAsStringAsync();
                _logger.LogTrace(readAsStringAsync);
                _logger.LogInformation(readAsStringAsync);
                var result = await response.Content.ReadFromJsonAsync<FlutterWaveVerificationResponse>();
                if (result is { Data.Amount: > 0 })
                {
                    if (result.Data.TxRef != verifyTransactionDto.TrxRef &&
                        Convert.ToDecimal(result.Data.Amount) < verifyTransactionDto.Amount)
                    {
                        result.Status = "failed. data not same";

                        _logger.LogTrace($"save to db FlutterwaveVerificationResponses {result}");
                        _logger.LogInformation($"save to db FlutterwaveVerificationResponses {result}");

                        try
                        {
                            await context.FlutterWaveVerificationResponses.AddAsync(result);
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
                        {
                            _logger.LogTrace("error",ex);
                            _logger.LogInformation("error",ex);
                        }

                        return false;
                    }
                    else
                    {
                        result.Status = "success";

                        _logger.LogTrace($"save to db FlutterwaveVerificationResponses {result}");
                        _logger.LogInformation($"save to db FlutterwaveVerificationResponses {result}");
                        
                        if (await DoesTrnxRefExists(verifyTransactionDto.TrxRef))
                        {
                            _logger.LogTrace(
                                $"again already exist so not verifying {verifyTransactionDto.TransactionId}");
                            return true;
                        }
                        
                        try
                        {
                            var getPropertyBooking =
                                await context.PropertyBookings.FirstOrDefaultAsync(p => p.Id == verifyTransactionDto.RelatedEntityId);
                            getPropertyBooking.IsPaid = true;
                            getPropertyBooking.DatePaid = DateTimeOffset.Now;
                            context.PropertyBookings.Update(getPropertyBooking);
                            await context.FlutterWaveVerificationResponses.AddAsync(result);
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
                        {
                            _logger.LogTrace("error",ex);
                            _logger.LogInformation("error",ex);

                            return true;
                        }
                        
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogTrace(ex.Message, ex);
            }
        }
        else
        {
            var why = await response.Content.ReadAsStringAsync();
            Console.WriteLine(why);
        }

        return false;
    }

    private string? GetFlutterwaveKey()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            return Environment.GetEnvironmentVariable("HomxlyFlutterwaveDevSecretKey");
        }
        else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            return Environment.GetEnvironmentVariable("HomxlyFlutterwaveProdSecretKey");
        }
        else
        {
            return null;
        }
    }
    private async Task<bool> DoesTrnxRefExists(string trxRef)
    {
        return await context.FlutterWaveVerificationResponses.FirstOrDefaultAsync(x =>
            x.Data.TxRef.ToLower() == trxRef.ToLower()) != null;
    }

    private bool IsDuplicateKeyException(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("duplicate key value violates unique constraint") ?? false;
    }
}