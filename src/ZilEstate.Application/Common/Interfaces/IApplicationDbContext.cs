using Microsoft.EntityFrameworkCore;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Property> Properties { get; }
    DbSet<User> Users { get; }
    DbSet<Agency> Agencies { get; }
    DbSet<PropertyImage> PropertyImages { get; }
    DbSet<Location> Locations { get; }
    DbSet<Review> Reviews { get; }
    DbSet<PropertyQuestion> Questions { get; }
    DbSet<PriceAlert> PriceAlerts { get; }
    DbSet<SavedSearch> SavedSearches { get; }
    DbSet<ViewingRequest> ViewingRequests { get; }
    DbSet<Inquiry> Inquiries { get; }
    DbSet<AgencyReview> AgencyReviews { get; }
    DbSet<PriceHistory> PriceHistories { get; }
    DbSet<Collection> Collections { get; }
    DbSet<CollectionItem> CollectionItems { get; }
    DbSet<OpenHouseEvent> OpenHouseEvents { get; }
    DbSet<OpenHouseRsvp> OpenHouseRsvps { get; }
    DbSet<PropertyReport> PropertyReports { get; }
    DbSet<PropertyTracking> PropertyTrackings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
