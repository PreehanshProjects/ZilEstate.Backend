using Microsoft.AspNetCore.Mvc;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AmenitiesController : ControllerBase
{
    private static readonly List<AmenityItem> AllAmenities = new()
    {
        new("Victoria Hospital",          "Hospital",    -20.3156, 57.5264),
        new("Dr Jeetoo Hospital",          "Hospital",    -20.1609, 57.4977),
        new("Fortis Clinique Darné",       "Hospital",    -20.3120, 57.5280),
        new("Apollo Bramwell Hospital",    "Hospital",    -20.2390, 57.4900),
        new("SSR International Airport",   "Airport",     -20.4301, 57.6836),
        new("Super U Rose Hill",           "Supermarket", -20.2391, 57.4629),
        new("Winner's Quatre Bornes",      "Supermarket", -20.2654, 57.4804),
        new("Jumbo Score Riche Terre",     "Supermarket", -20.0950, 57.5560),
        new("Happy World Grand Baie",      "Supermarket", -20.0087, 57.5831),
        new("Intermart Tamarin",           "Supermarket", -20.3281, 57.3769),
        new("Royal College Port Louis",    "School",      -20.1609, 57.4977),
        new("Curepipe College",            "School",      -20.3156, 57.5264),
        new("Lycée Labourdonnais",         "School",      -20.1700, 57.5050),
        new("Grand Baie La Croisette",     "Mall",        -20.0087, 57.5831),
        new("Bagatelle Mall",              "Mall",        -20.2760, 57.4800),
        new("Caudan Waterfront",           "Mall",        -20.1609, 57.4977),
        new("Trianon Shopping Park",       "Mall",        -20.2550, 57.4750),
        new("Flic en Flac Beach",          "Beach",       -20.2780, 57.3680),
        new("Grand Baie Beach",            "Beach",       -20.0087, 57.5831),
        new("Blue Bay Beach",              "Beach",       -20.4480, 57.7148),
        new("Belle Mare Beach",            "Beach",       -20.1919, 57.7672),
        new("Trou aux Biches Beach",       "Beach",       -19.9943, 57.5420),
        new("Le Morne Beach",              "Beach",       -20.4977, 57.3662),
        new("Mont Choisy Beach",           "Beach",       -20.0300, 57.5620),
        new("Chamarel Waterfall",          "Attraction",  -20.4299, 57.3878),
        new("Black River Gorges NP",       "Park",        -20.3900, 57.4300),
        new("Sir Seewoosagur Ramgoolam Botanical Garden", "Park", -20.0986, 57.5756),
        new("Mahébourg Waterfront Market", "Attraction",  -20.4078, 57.7029),
        new("Central Market Port Louis",   "Market",      -20.1609, 57.4977),
        new("Flacq Market",                "Market",      -20.1883, 57.7145)
    };

    [HttpGet]
    public IActionResult GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radiusKm = 5.0)
    {
        var results = AllAmenities
            .Select(a => new AmenityDto(
                a.Name,
                a.Category,
                a.Latitude,
                a.Longitude,
                Math.Round(Haversine(lat, lng, a.Latitude, a.Longitude), 2)))
            .Where(a => a.DistanceKm <= radiusKm)
            .OrderBy(a => a.DistanceKm)
            .ToList();

        return Ok(results);
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private record AmenityItem(string Name, string Category, double Latitude, double Longitude);
}

public record AmenityDto(string Name, string Category, double Latitude, double Longitude, double DistanceKm);
