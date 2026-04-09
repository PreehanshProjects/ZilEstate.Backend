namespace ZilEstate.Application.DTOs;

public class ValuationResultDto
{
    public decimal EstimatedLow { get; set; }
    public decimal EstimatedMid { get; set; }
    public decimal EstimatedHigh { get; set; }
    public decimal AvgPricePerM2 { get; set; }
    public int ComparableCount { get; set; }
    public string District { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class ListingScoreDto
{
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public string Grade { get; set; } = string.Empty;
    public List<ScoreItem> Items { get; set; } = new();
}

public class ScoreItem
{
    public string Label { get; set; } = string.Empty;
    public bool Achieved { get; set; }
    public int Points { get; set; }
}
