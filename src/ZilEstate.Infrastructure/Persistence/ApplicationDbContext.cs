using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Domain.Entities;
using ZilEstate.Domain.Enums;
using ZilEstate.Infrastructure.Persistence.Configurations;

namespace ZilEstate.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Agency> Agencies => Set<Agency>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<PropertyQuestion> Questions => Set<PropertyQuestion>();
    public DbSet<PriceAlert> PriceAlerts => Set<PriceAlert>();
    public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();
    public DbSet<ViewingRequest> ViewingRequests => Set<ViewingRequest>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<AgencyReview> AgencyReviews => Set<AgencyReview>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();
    public DbSet<OpenHouseEvent> OpenHouseEvents => Set<OpenHouseEvent>();
    public DbSet<OpenHouseRsvp> OpenHouseRsvps => Set<OpenHouseRsvp>();
    public DbSet<PropertyReport> PropertyReports => Set<PropertyReport>();
    public DbSet<PropertyTracking> PropertyTrackings => Set<PropertyTracking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());

        modelBuilder.Entity<Agency>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Name).HasMaxLength(200).IsRequired();
            b.Property(a => a.Description).HasMaxLength(2000);
            b.Property(a => a.Email).HasMaxLength(200);
            b.Property(a => a.Phone).HasMaxLength(20);
            b.HasMany(a => a.Properties).WithOne(p => p.Agency).HasForeignKey(p => p.AgencyId).OnDelete(DeleteBehavior.SetNull);
            b.HasMany(a => a.Agents).WithOne(u => u.Agency).HasForeignKey(u => u.AgencyId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Email).HasMaxLength(200).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            b.Property(u => u.Role).HasMaxLength(20).IsRequired();
            b.Property(u => u.IsVerified).HasDefaultValue(false);
            b.HasMany(u => u.Properties).WithOne(p => p.User).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Review>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.AuthorName).HasMaxLength(100).IsRequired();
            b.Property(r => r.Comment).HasMaxLength(2000).IsRequired();
            b.HasOne(r => r.Property).WithMany(p => p.Reviews).HasForeignKey(r => r.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PropertyQuestion>(b =>
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.AuthorName).HasMaxLength(100).IsRequired();
            b.Property(q => q.Question).HasMaxLength(1000).IsRequired();
            b.Property(q => q.Answer).HasMaxLength(2000);
            b.HasOne(q => q.Property).WithMany(p => p.Questions).HasForeignKey(q => q.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PriceAlert>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Status).HasMaxLength(20).IsRequired();
            b.Property(a => a.District).HasMaxLength(100);
            b.Property(a => a.PropertyType).HasMaxLength(50);
            b.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SavedSearch>(b => {
            b.HasKey(s => s.Id);
            b.Property(s => s.Name).HasMaxLength(200).IsRequired();
            b.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ViewingRequest>(b => {
            b.HasKey(v => v.Id);
            b.Property(v => v.ContactName).HasMaxLength(100).IsRequired();
            b.Property(v => v.ContactEmail).HasMaxLength(200).IsRequired();
            b.Property(v => v.ContactPhone).HasMaxLength(20).IsRequired();
            b.Property(v => v.Status).HasMaxLength(20).IsRequired();
            b.HasOne(v => v.Property).WithMany().HasForeignKey(v => v.PropertyId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(v => v.User).WithMany().HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Inquiry>(b => {
            b.HasKey(i => i.Id);
            b.Property(i => i.ContactName).HasMaxLength(100).IsRequired();
            b.Property(i => i.ContactEmail).HasMaxLength(200).IsRequired();
            b.Property(i => i.ContactPhone).HasMaxLength(20).IsRequired();
            b.Property(i => i.Message).HasMaxLength(2000).IsRequired();
            b.HasOne(i => i.Property).WithMany().HasForeignKey(i => i.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AgencyReview>(b => {
            b.HasKey(r => r.Id);
            b.Property(r => r.AuthorName).HasMaxLength(100).IsRequired();
            b.Property(r => r.Comment).HasMaxLength(2000).IsRequired();
            b.HasOne(r => r.Agency).WithMany(a => a.Reviews).HasForeignKey(r => r.AgencyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PriceHistory>(b => {
            b.HasKey(h => h.Id);
            b.Property(h => h.Note).HasMaxLength(200);
            b.HasOne(h => h.Property).WithMany(p => p.PriceHistories).HasForeignKey(h => h.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Collection>(b => {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).HasMaxLength(100).IsRequired();
            b.Property(c => c.Description).HasMaxLength(500);
            b.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CollectionItem>(b => {
            b.HasKey(ci => ci.Id);
            b.HasIndex(ci => new { ci.CollectionId, ci.PropertyId }).IsUnique();
            b.HasOne(ci => ci.Collection).WithMany(c => c.Items).HasForeignKey(ci => ci.CollectionId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(ci => ci.Property).WithMany().HasForeignKey(ci => ci.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OpenHouseEvent>(b => {
            b.HasKey(e => e.Id);
            b.Property(e => e.Description).HasMaxLength(1000);
            b.HasOne(e => e.Property).WithMany().HasForeignKey(e => e.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OpenHouseRsvp>(b => {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name).HasMaxLength(100).IsRequired();
            b.Property(r => r.Email).HasMaxLength(200).IsRequired();
            b.Property(r => r.Phone).HasMaxLength(20);
            b.HasOne(r => r.Event).WithMany(e => e.Rsvps).HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PropertyReport>(b => {
            b.HasKey(r => r.Id);
            b.Property(r => r.ReporterName).HasMaxLength(100).IsRequired();
            b.Property(r => r.ReporterEmail).HasMaxLength(200).IsRequired();
            b.Property(r => r.Reason).HasMaxLength(50).IsRequired();
            b.Property(r => r.Details).HasMaxLength(2000);
            b.Property(r => r.Status).HasMaxLength(20).IsRequired();
            b.HasOne(r => r.Property).WithMany().HasForeignKey(r => r.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PropertyTracking>(b => {
            b.HasKey(t => t.Id);
            b.Property(t => t.TrackedPrice).HasColumnType("decimal(18,2)");
            b.HasIndex(t => new { t.UserId, t.PropertyId }).IsUnique();
            b.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(t => t.Property).WithMany().HasForeignKey(t => t.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Mauritius locations
        modelBuilder.Entity<Location>().HasData(
            new Location { Id = 1,  District = "Port Louis",         City = "Port Louis",       Latitude = -20.1609, Longitude = 57.4977 },
            new Location { Id = 2,  District = "Plaines Wilhems",    City = "Curepipe",         Latitude = -20.3156, Longitude = 57.5264 },
            new Location { Id = 3,  District = "Plaines Wilhems",    City = "Quatre Bornes",    Latitude = -20.2654, Longitude = 57.4804 },
            new Location { Id = 4,  District = "Plaines Wilhems",    City = "Vacoas-Phoenix",   Latitude = -20.2980, Longitude = 57.4953 },
            new Location { Id = 5,  District = "Plaines Wilhems",    City = "Rose Hill",        Latitude = -20.2391, Longitude = 57.4629 },
            new Location { Id = 6,  District = "Black River",        City = "Tamarin",          Latitude = -20.3281, Longitude = 57.3769 },
            new Location { Id = 7,  District = "Black River",        City = "Flic en Flac",     Latitude = -20.2780, Longitude = 57.3680 },
            new Location { Id = 8,  District = "Black River",        City = "La Gaulette",      Latitude = -20.4977, Longitude = 57.3662 },
            new Location { Id = 9,  District = "Rivière du Rempart", City = "Grand Baie",       Latitude = -20.0087, Longitude = 57.5831 },
            new Location { Id = 10, District = "Pamplemousses",      City = "Pamplemousses",    Latitude = -20.0986, Longitude = 57.5756 },
            new Location { Id = 11, District = "Flacq",              City = "Centre de Flacq",  Latitude = -20.1883, Longitude = 57.7145 },
            new Location { Id = 12, District = "Grand Port",         City = "Mahébourg",        Latitude = -20.4078, Longitude = 57.7029 },
            new Location { Id = 13, District = "Savanne",            City = "Souillac",         Latitude = -20.5201, Longitude = 57.5139 },
            new Location { Id = 14, District = "Moka",               City = "Moka",             Latitude = -20.2256, Longitude = 57.5381 },
            new Location { Id = 15, District = "Rivière du Rempart", City = "Triolet",          Latitude = -20.0575, Longitude = 57.5420 },
            new Location { Id = 16, District = "Black River",        City = "Chamarel",         Latitude = -20.4299, Longitude = 57.3878 },
            new Location { Id = 17, District = "Rivière du Rempart", City = "Goodlands",        Latitude = -19.9943, Longitude = 57.6444 },
            new Location { Id = 18, District = "Flacq",              City = "Belle Mare",       Latitude = -20.1919, Longitude = 57.7672 },
            new Location { Id = 19, District = "Grand Port",         City = "Blue Bay",         Latitude = -20.4480, Longitude = 57.7148 },
            new Location { Id = 20, District = "Pamplemousses",      City = "Terre Rouge",      Latitude = -20.1456, Longitude = 57.5508 }
        );

        // Seed sample agencies
        modelBuilder.Entity<Agency>().HasData(
            new Agency {
                Id = 1,
                Name = "Luxury Mauritius Properties",
                Description = "Specializing in high-end luxury villas and beachfront estates across Mauritius. Our team of experts provides personalized service to help you find your dream home.",
                LogoUrl = "https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=200",
                Website = "https://luxurymauritius.mu",
                Email = "info@luxurymauritius.mu",
                Phone = "+230 5250 1000",
                Address = "Grand Baie La Croisette, Grand Baie",
                IsVerified = true,
                Plan = "Professional",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Agency {
                Id = 2,
                Name = "Paradise Island Real Estate",
                Description = "Your trusted partner for residential and commercial properties in Mauritius. We cover all districts and offer a wide range of options for every budget.",
                LogoUrl = "https://images.unsplash.com/photo-1560520653-9e0e4c89eb11?w=200",
                Website = "https://paradiseisland.mu",
                Email = "contact@paradiseisland.mu",
                Phone = "+230 5450 2000",
                Address = "Ebene Cybercity, Ebene",
                IsVerified = true,
                Plan = "Professional",
                CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed sample properties
        modelBuilder.Entity<Property>().HasData(
            new Property { Id = 1, Title = "Luxury Villa in Grand Baie", Description = "Stunning 4-bedroom villa with private pool and sea view. Perfect for families looking for luxury living in the north of Mauritius.", Price = 15_000_000, Type = PropertyType.Villa, Status = PropertyStatus.ForSale, Bedrooms = 4, Bathrooms = 3, Parking = 2, SizeM2 = 350, Furnished = true, SellerName = "Jean-Marc Dupont", SellerPhone = "+230 5742 8834", SellerWhatsApp = "+23057428834", SellerEmail = "jmdupont@email.com", LocationId = 9, Latitude = -20.0087, Longitude = 57.5831, IsApproved = true, IsFeatured = true, CreatedAt = new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc), AgencyId = 1, ExpiresAt = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Property { Id = 2, Title = "Modern Apartment in Quatre Bornes", Description = "Bright 2-bedroom apartment on the 3rd floor with balcony. Close to supermarkets, schools, and the city center.", Price = 3_500_000, Type = PropertyType.Apartment, Status = PropertyStatus.ForSale, Bedrooms = 2, Bathrooms = 1, Parking = 1, SizeM2 = 85, Furnished = false, SellerName = "Priya Ramgoolam", SellerPhone = "+230 5251 9012", SellerWhatsApp = "+23052519012", SellerEmail = "priya.r@email.com", LocationId = 3, Latitude = -20.2654, Longitude = 57.4804, IsApproved = true, IsFeatured = true, CreatedAt = new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Utc), AgencyId = 2, ExpiresAt = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Property { Id = 3, Title = "Land Plot in Tamarin - Sea View", Description = "Rare 15-arpent plot with panoramic sea view in the exclusive Tamarin area. Building permit available.", Price = 8_500_000, Type = PropertyType.Land, Status = PropertyStatus.ForSale, SizeArpents = 15, SellerName = "Antoine Leclercq", SellerPhone = "+230 5888 2345", SellerWhatsApp = "+23058882345", LocationId = 6, Latitude = -20.3281, Longitude = 57.3769, IsApproved = true, IsFeatured = false, CreatedAt = new DateTime(2026, 3, 22, 0, 0, 0, DateTimeKind.Utc), AgencyId = 1, ExpiresAt = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Property { Id = 4, Title = "House for Rent in Curepipe", Description = "Spacious 3-bedroom house available for long-term rental. Quiet neighborhood, fully furnished, with garden and covered parking.", Price = 35_000, Type = PropertyType.House, Status = PropertyStatus.ForRent, Bedrooms = 3, Bathrooms = 2, Parking = 1, SizeM2 = 200, Furnished = true, SellerName = "Marie-Claire Bonnefoy", SellerPhone = "+230 5314 6789", SellerEmail = "mc.bonnefoy@email.com", LocationId = 2, Latitude = -20.3156, Longitude = 57.5264, IsApproved = true, IsFeatured = true, CreatedAt = new DateTime(2026, 3, 30, 0, 0, 0, DateTimeKind.Utc), ExpiresAt = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Property { Id = 5, Title = "Commercial Space in Port Louis", Description = "Prime commercial space in the heart of Port Louis business district. Ground floor, high visibility, ideal for retail or office.", Price = 45_000, Type = PropertyType.Commercial, Status = PropertyStatus.ForRent, SizeM2 = 120, SellerName = "Karim Bhunjun", SellerPhone = "+230 5456 7890", SellerWhatsApp = "+23054567890", SellerEmail = "karim.b@business.mu", LocationId = 1, Latitude = -20.1609, Longitude = 57.4977, IsApproved = true, IsFeatured = false, CreatedAt = new DateTime(2026, 3, 25, 0, 0, 0, DateTimeKind.Utc), AgencyId = 2, ExpiresAt = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Property { Id = 6, Title = "Beachfront Villa in Flic en Flac", Description = "Exceptional beachfront villa with direct beach access. 5 bedrooms, private infinity pool, fully equipped kitchen and outdoor BBQ area.", Price = 25_000_000, Type = PropertyType.Villa, Status = PropertyStatus.ForSale, Bedrooms = 5, Bathrooms = 4, Parking = 3, SizeM2 = 500, Furnished = true, SellerName = "Sophie Bertrand", SellerPhone = "+230 5567 2345", SellerWhatsApp = "+23055672345", SellerEmail = "sophie.b@lux.mu", LocationId = 7, Latitude = -20.2780, Longitude = 57.3680, IsApproved = true, IsFeatured = true, CreatedAt = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc), AgencyId = 1, ExpiresAt = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<PropertyImage>().HasData(
            new PropertyImage { Id = 1, Url = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800", IsPrimary = true,  PropertyId = 1 },
            new PropertyImage { Id = 2, Url = "https://images.unsplash.com/photo-1613977257363-707ba9348227?w=800", IsPrimary = false, PropertyId = 1 },
            new PropertyImage { Id = 3, Url = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800", IsPrimary = true,  PropertyId = 2 },
            new PropertyImage { Id = 4, Url = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800", IsPrimary = false, PropertyId = 2 },
            new PropertyImage { Id = 5, Url = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=800", IsPrimary = true,  PropertyId = 3 },
            new PropertyImage { Id = 6, Url = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800", IsPrimary = true,  PropertyId = 4 },
            new PropertyImage { Id = 7, Url = "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800", IsPrimary = true,  PropertyId = 5 },
            new PropertyImage { Id = 8, Url = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800", IsPrimary = true,  PropertyId = 6 },
            new PropertyImage { Id = 9, Url = "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800", IsPrimary = false, PropertyId = 6 }
        );
    }
}
