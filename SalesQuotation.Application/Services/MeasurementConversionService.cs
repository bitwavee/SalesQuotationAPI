using Microsoft.Extensions.Logging;
using SalesQuotation.Application.Services;

namespace SalesQuotation.Application.Services;

/// <summary>
/// Implementation of IMeasurementConversionService
/// </summary>
public class MeasurementConversionService : IMeasurementConversionService
{
    private readonly ILogger<MeasurementConversionService> _logger;

    // Conversion constants
    private const decimal METER_TO_FEET = 3.28084m;
    private const decimal FEET_TO_METER = 0.3048m;
    private const decimal SQUARE_METER_TO_SQUARE_FEET = 10.7639m;
    private const decimal SQUARE_FEET_TO_SQUARE_METER = 0.092903m;

    public MeasurementConversionService(ILogger<MeasurementConversionService> logger)
    {
        _logger = logger;
    }

    public decimal ConvertMeterToSquareFeet(decimal length, decimal breadth)
    {
        _logger.LogDebug($"Converting {length}m x {breadth}m to square feet");
        
        decimal squareMeter = length * breadth;
        decimal squareFeet = squareMeter * SQUARE_METER_TO_SQUARE_FEET;

        _logger.LogDebug($"Conversion result: {squareFeet} sq ft");
        
        return Math.Round(squareFeet, 2);
    }

    public decimal ConvertSquareFeetToMeter(decimal squareFeet)
    {
        _logger.LogDebug($"Converting {squareFeet} sq ft to square meters");
        
        decimal squareMeter = squareFeet * SQUARE_FEET_TO_SQUARE_METER;
        
        _logger.LogDebug($"Conversion result: {squareMeter} sq m");
        
        return Math.Round(squareMeter, 2);
    }

    public decimal ConvertMeterToFeet(decimal meters)
    {
        _logger.LogDebug($"Converting {meters}m to feet");
        
        decimal feet = meters * METER_TO_FEET;
        
        _logger.LogDebug($"Conversion result: {feet} ft");
        
        return Math.Round(feet, 2);
    }

    public decimal ConvertFeetToMeter(decimal feet)
    {
        _logger.LogDebug($"Converting {feet} ft to meters");
        
        decimal meters = feet * FEET_TO_METER;
        
        _logger.LogDebug($"Conversion result: {meters} m");
        
        return Math.Round(meters, 2);
    }
}
