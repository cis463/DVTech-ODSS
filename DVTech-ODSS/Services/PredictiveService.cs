using DVTech_ODSS.Data;
using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Services
{
    public class PredictiveService
    {
        private readonly ApplicationDbContext _context;

        public PredictiveService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Predict demand for a specific date
        public async Task<List<DemandForecast>> PredictDemandForDateAsync(DateTime targetDate)
        {
            var forecasts = new List<DemandForecast>();
            var allSales = await GetHistoricalSalesDataAsync();

            if (!allSales.Any())
                return forecasts;

            var itemGroups = allSales.GroupBy(s => new { s.ItemId, s.ItemName, s.Category });

            foreach (var group in itemGroups)
            {
                var forecast = CalculateForecast(group.ToList(), targetDate);
                if (forecast != null)
                {
                    forecasts.Add(forecast);
                }
            }

            return forecasts.OrderByDescending(f => f.PredictedDemand).ToList();
        }

        // Predict demand for a season (Quarter)
        public async Task<List<DemandForecast>> PredictDemandForSeasonAsync(int year, int quarter)
        {
            var forecasts = new List<DemandForecast>();
            var allSales = await GetHistoricalSalesDataAsync();

            if (!allSales.Any())
                return forecasts;

            var itemGroups = allSales.GroupBy(s => new { s.ItemId, s.ItemName, s.Category });

            foreach (var group in itemGroups)
            {
                var forecast = CalculateSeasonalForecast(group.ToList(), year, quarter);
                if (forecast != null)
                {
                    forecasts.Add(forecast);
                }
            }

            return forecasts.OrderByDescending(f => f.PredictedDemand).ToList();
        }

        // Get top selling items
        public async Task<List<DemandForecast>> GetTopSellingItemsAsync(int topCount = 10)
        {
            var allSales = await GetHistoricalSalesDataAsync();

            if (!allSales.Any())
                return new List<DemandForecast>();

            var topItems = allSales
                .GroupBy(s => new { s.ItemId, s.ItemName, s.Category })
                .Select(g => new DemandForecast
                {
                    ItemId = g.Key.ItemId,
                    ItemName = g.Key.ItemName,
                    Category = g.Key.Category,
                    PredictedDemand = (int)g.Sum(s => s.Quantity),
                    ConfidenceLevel = 100,
                    TrendIndicator = "High"
                })
                .OrderByDescending(f => f.PredictedDemand)
                .Take(topCount)
                .ToList();

            return topItems;
        }

        // Get seasonal trends
        public async Task<Dictionary<string, List<SeasonalTrend>>> GetSeasonalTrendsAsync()
        {
            var allSales = await GetHistoricalSalesDataAsync();
            var trends = new Dictionary<string, List<SeasonalTrend>>();

            var itemGroups = allSales.GroupBy(s => s.ItemName);

            foreach (var group in itemGroups)
            {
                var itemTrends = new List<SeasonalTrend>();

                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    var quarterSales = group.Where(s => GetQuarter(s.SaleDate) == quarter);
                    var avgDemand = quarterSales.Any() ? quarterSales.Average(s => s.Quantity) : 0;

                    itemTrends.Add(new SeasonalTrend
                    {
                        Quarter = quarter,
                        QuarterName = GetQuarterName(quarter),
                        AverageDemand = avgDemand,
                        TotalSales = quarterSales.Sum(s => s.Quantity)
                    });
                }

                trends[group.Key] = itemTrends;
            }

            return trends;
        }

        // PRIVATE HELPER METHODS

        private async Task<List<SaleItemData>> GetHistoricalSalesDataAsync()
        {
            var sales = await _context.SaleItems
                .Include(si => si.Sale)
                .Include(si => si.InventoryItem)
                .Where(si => si.Sale != null && si.Sale.IsActive)
                .Select(si => new SaleItemData
                {
                    ItemId = si.ItemId,
                    ItemName = si.ItemName,
                    Category = si.Category,
                    Quantity = si.Quantity,
                    SaleDate = si.Sale.SaleDate,
                    TotalPrice = si.TotalPrice
                })
                .ToListAsync();

            return sales;
        }

        private DemandForecast? CalculateForecast(List<SaleItemData> historicalData, DateTime targetDate)
        {
            if (!historicalData.Any())
                return null;

            var itemId = historicalData.First().ItemId;
            var itemName = historicalData.First().ItemName;
            var category = historicalData.First().Category;

            // Simple Moving Average with seasonal adjustment
            var recentSales = historicalData
                .Where(s => s.SaleDate >= targetDate.AddMonths(-6))
                .ToList();

            if (!recentSales.Any())
            {
                recentSales = historicalData.OrderByDescending(s => s.SaleDate).Take(10).ToList();
            }

            var avgDemand = recentSales.Average(s => s.Quantity);

            // Seasonal adjustment
            var targetQuarter = GetQuarter(targetDate);
            var historicalQuarterSales = historicalData
                .Where(s => GetQuarter(s.SaleDate) == targetQuarter)
                .ToList();

            double seasonalFactor = 1.0;
            if (historicalQuarterSales.Any())
            {
                var quarterAvg = historicalQuarterSales.Average(s => s.Quantity);
                var overallAvg = historicalData.Average(s => s.Quantity);
                seasonalFactor = overallAvg > 0 ? quarterAvg / overallAvg : 1.0;
            }

            var predictedDemand = (int)Math.Ceiling(avgDemand * seasonalFactor);

            // Calculate confidence based on data points
            var confidence = CalculateConfidence(historicalData.Count, recentSales.Count);

            // Determine trend
            var trend = DetermineTrend(historicalData);

            return new DemandForecast
            {
                ItemId = itemId,
                ItemName = itemName,
                Category = category,
                PredictedDemand = predictedDemand,
                ConfidenceLevel = confidence,
                TrendIndicator = trend,
                ForecastDate = targetDate,
                DataPointsUsed = historicalData.Count
            };
        }

        private DemandForecast? CalculateSeasonalForecast(List<SaleItemData> historicalData, int year, int quarter)
        {
            if (!historicalData.Any())
                return null;

            var itemId = historicalData.First().ItemId;
            var itemName = historicalData.First().ItemName;
            var category = historicalData.First().Category;

            // Get historical data for the same quarter
            var sameQuarterData = historicalData
                .Where(s => GetQuarter(s.SaleDate) == quarter)
                .ToList();

            if (!sameQuarterData.Any())
                return null;

            var avgDemand = sameQuarterData.Average(s => s.Quantity);

            // Apply growth trend if available
            var yearlyData = historicalData
                .GroupBy(s => s.SaleDate.Year)
                .OrderBy(g => g.Key)
                .Select(g => new { Year = g.Key, Total = g.Sum(s => s.Quantity) })
                .ToList();

            double growthRate = 0;
            if (yearlyData.Count >= 2)
            {
                var firstYear = yearlyData.First().Total;
                var lastYear = yearlyData.Last().Total;
                growthRate = firstYear > 0 ? (lastYear - firstYear) / firstYear / (yearlyData.Count - 1) : 0;
            }

            var predictedDemand = (int)Math.Ceiling(avgDemand * (1 + growthRate));

            var confidence = CalculateConfidence(historicalData.Count, sameQuarterData.Count);
            var trend = DetermineTrend(historicalData);

            return new DemandForecast
            {
                ItemId = itemId,
                ItemName = itemName,
                Category = category,
                PredictedDemand = predictedDemand,
                ConfidenceLevel = confidence,
                TrendIndicator = trend,
                ForecastQuarter = quarter,
                ForecastYear = year,
                DataPointsUsed = historicalData.Count
            };
        }

        private int CalculateConfidence(int totalDataPoints, int recentDataPoints)
        {
            if (totalDataPoints == 0)
                return 0;

            // Base confidence on number of data points
            int confidence = 0;

            if (totalDataPoints >= 50)
                confidence = 90;
            else if (totalDataPoints >= 30)
                confidence = 75;
            else if (totalDataPoints >= 15)
                confidence = 60;
            else if (totalDataPoints >= 5)
                confidence = 45;
            else
                confidence = 30;

            // Adjust based on recent data availability
            if (recentDataPoints < 3)
                confidence -= 10;

            return Math.Max(30, Math.Min(95, confidence));
        }

        private string DetermineTrend(List<SaleItemData> historicalData)
        {
            if (historicalData.Count < 6)
                return "Stable";

            var sortedData = historicalData.OrderBy(s => s.SaleDate).ToList();
            var halfwayPoint = sortedData.Count / 2;

            var firstHalf = sortedData.Take(halfwayPoint).Average(s => s.Quantity);
            var secondHalf = sortedData.Skip(halfwayPoint).Average(s => s.Quantity);

            var change = secondHalf - firstHalf;
            var percentChange = firstHalf > 0 ? (change / firstHalf) * 100 : 0;

            if (percentChange > 15)
                return "Rising";
            else if (percentChange < -15)
                return "Declining";
            else
                return "Stable";
        }

        private int GetQuarter(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        private string GetQuarterName(int quarter)
        {
            return quarter switch
            {
                1 => "Q1 (Jan-Mar)",
                2 => "Q2 (Apr-Jun)",
                3 => "Q3 (Jul-Sep)",
                4 => "Q4 (Oct-Dec)",
                _ => "Unknown"
            };
        }
    }

    // REMOVED: All class definitions (DemandForecast, SaleItemData, SeasonalTrend)
    // These should ONLY be in Models/DemandForecast.cs
}