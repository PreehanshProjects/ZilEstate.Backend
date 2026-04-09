using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private static readonly string[] Districts =
    {
        "Port Louis", "Pamplemousses", "Rivière du Rempart", "Flacq",
        "Grand Port", "Savanne", "Black River", "Plaines Wilhems", "Moka"
    };

    private static readonly string[] Cities =
    {
        "Grand Baie", "Tamarin", "Flic en Flac", "Curepipe", "Quatre Bornes",
        "Rose Hill", "Mahébourg", "Blue Bay", "Belle Mare", "Triolet",
        "Goodlands", "Chamarel", "Moka", "Souillac"
    };

    // Templates per type: 5 templates each
    private static readonly Dictionary<string, string[]> DescriptionTemplates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Villa"] = new[]
        {
            "Nestled in the heart of {location}, this stunning {size}m² villa offers an unparalleled lifestyle. With {bedrooms} spacious bedrooms and {bathrooms} bathrooms, it is perfectly designed for comfort and elegance. Priced at MUR {price:N0}, this property is a rare gem. {features}",
            "Welcome to this magnificent villa in {location}. Spanning {size}m², this {bedrooms}-bedroom, {bathrooms}-bathroom residence combines modern design with island living. Available at MUR {price:N0}. {features}",
            "Discover luxury living in {location} with this exquisite {size}m² villa. Featuring {bedrooms} bedrooms and {bathrooms} well-appointed bathrooms, this property redefines comfort at MUR {price:N0}. {features}",
            "This impressive {bedrooms}-bedroom villa in {location} spans {size}m² and offers {bathrooms} bathrooms alongside premium finishes throughout. Listed at MUR {price:N0}. {features}",
            "Set in the desirable area of {location}, this {size}m² villa boasts {bedrooms} bedrooms, {bathrooms} bathrooms, and a lifestyle to match. Asking price: MUR {price:N0}. {features}"
        },
        ["House"] = new[]
        {
            "A wonderful family home in {location}, this {size}m² property features {bedrooms} bedrooms and {bathrooms} bathrooms. Ideal for those seeking comfort and convenience at MUR {price:N0}. {features}",
            "This charming {bedrooms}-bedroom house in {location} offers {size}m² of living space with {bathrooms} bathrooms. A perfect family residence priced at MUR {price:N0}. {features}",
            "Located in {location}, this spacious {size}m² house with {bedrooms} bedrooms and {bathrooms} bathrooms offers everything a modern family needs. Priced at MUR {price:N0}. {features}",
            "Step into this welcoming {bedrooms}-bedroom home in {location}. Covering {size}m² and featuring {bathrooms} bathrooms, it offers great value at MUR {price:N0}. {features}",
            "Enjoy comfortable living in this well-appointed house in {location}. The property spans {size}m² with {bedrooms} bedrooms and {bathrooms} bathrooms, listed at MUR {price:N0}. {features}"
        },
        ["Apartment"] = new[]
        {
            "This modern {bedrooms}-bedroom apartment in {location} offers {size}m² of stylish living space with {bathrooms} bathrooms. Priced attractively at MUR {price:N0}. {features}",
            "Enjoy urban convenience in this {size}m² apartment in {location}. With {bedrooms} bedrooms and {bathrooms} bathrooms, it is the ideal city retreat at MUR {price:N0}. {features}",
            "A superb apartment opportunity in {location}: {bedrooms} bedrooms, {bathrooms} bathrooms, and {size}m² of thoughtfully designed space at MUR {price:N0}. {features}",
            "Contemporary living awaits in this {bedrooms}-bedroom apartment in {location}. Spanning {size}m² with {bathrooms} bathrooms, it's priced at MUR {price:N0}. {features}",
            "This bright {size}m² apartment in {location} features {bedrooms} bedrooms and {bathrooms} bathrooms — the perfect urban home at MUR {price:N0}. {features}"
        },
        ["Land"] = new[]
        {
            "A rare opportunity to acquire a {size}m² plot of land in {location}. Priced at MUR {price:N0}, this is an ideal site for your dream project. {features}",
            "This {size}m² land parcel in {location} offers endless development potential. Secure your investment at MUR {price:N0}. {features}",
            "Prime land available in {location}: {size}m² of versatile space ready for development, listed at MUR {price:N0}. {features}",
            "Invest in this strategically located {size}m² plot in {location}, available at MUR {price:N0}. {features}",
            "Seize this opportunity to own a {size}m² plot in {location} at MUR {price:N0}. A blank canvas for your vision. {features}"
        },
        ["Commercial"] = new[]
        {
            "Prime commercial space in {location}: {size}m² of versatile premises ideal for retail or office use, available at MUR {price:N0}. {features}",
            "This {size}m² commercial property in {location} offers high visibility and accessibility, listed at MUR {price:N0}. {features}",
            "A strategic commercial opportunity in {location} — {size}m² of well-positioned space for your business at MUR {price:N0}. {features}",
            "Expand your business in {location} with this {size}m² commercial space, available at MUR {price:N0}. {features}",
            "Well-located commercial premises in {location} spanning {size}m². Listed at MUR {price:N0}, this property is ready for immediate use. {features}"
        }
    };

    [HttpPost("search")]
    public IActionResult ParseSearch([FromBody] SearchQueryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Query))
            return BadRequest(new { message = "Query is required" });

        var query = dto.Query.ToLowerInvariant();
        var result = new ParsedSearchDto();

        // Extract type
        if (Regex.IsMatch(query, @"\bvilla\b")) result.Type = "Villa";
        else if (Regex.IsMatch(query, @"\bhouse\b|\bhome\b")) result.Type = "House";
        else if (Regex.IsMatch(query, @"\bapartment\b|\bapt\b|\bflat\b")) result.Type = "Apartment";
        else if (Regex.IsMatch(query, @"\bland\b|\bplot\b")) result.Type = "Land";
        else if (Regex.IsMatch(query, @"\bcommercial\b|\boffice\b|\bshop\b|\bretail\b")) result.Type = "Commercial";

        // Extract status
        if (Regex.IsMatch(query, @"\bfor\s+rent\b|\bto\s+rent\b|\brental\b|\brent\b"))
            result.Status = "ForRent";
        else if (Regex.IsMatch(query, @"\bfor\s+sale\b|\btosale\b|\bbuy\b|\bsale\b"))
            result.Status = "ForSale";

        // Extract maxPrice: "under/below/max/less than X / XM / X million"
        var maxPriceMatch = Regex.Match(query, @"(?:under|below|max|less\s+than)\s*(?:rs\.?\s*)?(\d+(?:\.\d+)?)\s*(m|million|k)?");
        if (maxPriceMatch.Success)
        {
            result.MaxPrice = ParsePrice(maxPriceMatch.Groups[1].Value, maxPriceMatch.Groups[2].Value);
        }

        // Extract minPrice: "above/over/more than/at least X"
        var minPriceMatch = Regex.Match(query, @"(?:above|over|more\s+than|at\s+least|minimum|min)\s*(?:rs\.?\s*)?(\d+(?:\.\d+)?)\s*(m|million|k)?");
        if (minPriceMatch.Success)
        {
            result.MinPrice = ParsePrice(minPriceMatch.Groups[1].Value, minPriceMatch.Groups[2].Value);
        }

        // Extract bedrooms: "3 bed/bedroom/bedrooms"
        var bedroomMatch = Regex.Match(query, @"(\d+)\s*(?:-|–|—)?\s*(?:bed(?:room)?s?|br\b)");
        if (bedroomMatch.Success && int.TryParse(bedroomMatch.Groups[1].Value, out var beds))
            result.MinBedrooms = beds;

        // Extract size: "100m2", "150 sqm", "200 square meters"
        var sizeMatch = Regex.Match(query, @"(\d+(?:\.\d+)?)\s*(?:m2|sqm|m²|square\s*met(?:er|re)s?)");
        if (sizeMatch.Success && double.TryParse(sizeMatch.Groups[1].Value, out var size))
            result.MinSizeM2 = size;

        // Extract district/city (case-insensitive, original query)
        var originalQuery = dto.Query;
        foreach (var city in Cities)
        {
            if (originalQuery.IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.District = city;
                break;
            }
        }
        if (result.District == null)
        {
            foreach (var district in Districts)
            {
                if (originalQuery.IndexOf(district, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.District = district;
                    break;
                }
            }
        }

        return Ok(result);
    }

    [HttpPost("describe")]
    public IActionResult GenerateDescription([FromBody] PropertyDescribeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            return BadRequest(new { message = "Property type is required" });

        var type = dto.Type;
        if (!DescriptionTemplates.TryGetValue(type, out var templates))
            templates = DescriptionTemplates["House"];

        var bedrooms = dto.Bedrooms ?? 0;
        var size = (int)(dto.SizeM2 ?? 0);
        var templateIndex = (bedrooms + size) % 5;
        var template = templates[templateIndex];

        var featuresText = string.Empty;
        if (dto.Features != null && dto.Features.Count > 0)
        {
            var capitalised = dto.Features.Select(f =>
                f.Length > 0 ? char.ToUpper(f[0]) + f.Substring(1).ToLower() : f);
            featuresText = "Key features include: " + string.Join(", ", capitalised) + ".";
        }

        var description = template
            .Replace("{location}", dto.Location ?? "Mauritius")
            .Replace("{size}", size > 0 ? size.ToString() : "N/A")
            .Replace("{bedrooms}", bedrooms > 0 ? bedrooms.ToString() : "multiple")
            .Replace("{bathrooms}", dto.Bathrooms.HasValue ? dto.Bathrooms.Value.ToString() : "N/A")
            .Replace("{price:N0}", dto.Price.HasValue ? dto.Price.Value.ToString("N0") : "on request")
            .Replace("{features}", featuresText)
            .Trim();

        return Ok(new { description });
    }

    private static decimal ParsePrice(string numberStr, string suffix)
    {
        if (!decimal.TryParse(numberStr, out var number)) return 0;
        suffix = (suffix ?? "").ToLowerInvariant();
        return suffix switch
        {
            "m" or "million" => number * 1_000_000,
            "k" => number * 1_000,
            _ => number
        };
    }
}

public class SearchQueryDto
{
    public string Query { get; set; } = string.Empty;
}

public class ParsedSearchDto
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public string? District { get; set; }
    public double? MinSizeM2 { get; set; }
}

public class PropertyDescribeDto
{
    public string Type { get; set; } = string.Empty;
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public double? SizeM2 { get; set; }
    public string? Location { get; set; }
    public decimal? Price { get; set; }
    public List<string>? Features { get; set; }
}
