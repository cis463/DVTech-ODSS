namespace DVTech_ODSS.Models
{
    public class DemandForecast
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PredictedDemand { get; set; }
        public int ConfidenceLevel { get; set; }
        public string TrendIndicator { get; set; } = string.Empty;
        public DateTime? ForecastDate { get; set; }
        public int? ForecastQuarter { get; set; }
        public int? ForecastYear { get; set; }
        public int DataPointsUsed { get; set; }
    }

    public class SeasonalTrend
    {
        public int Quarter { get; set; }
        public string QuarterName { get; set; } = string.Empty;
        public double AverageDemand { get; set; }
        public int TotalSales { get; set; }
    }

    public class SaleItemData
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}