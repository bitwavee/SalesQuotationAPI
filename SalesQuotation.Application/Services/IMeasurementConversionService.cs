namespace SalesQuotation.Application.Services;

/// <summary>
/// Service for measurement conversions
/// </summary>
public interface IMeasurementConversionService
{
    /// <summary>
    /// Convert square meters to square feet
    /// </summary>
    decimal ConvertMeterToSquareFeet(decimal length, decimal breadth);

    /// <summary>
    /// Convert square feet to square meters
    /// </summary>
    decimal ConvertSquareFeetToMeter(decimal squareFeet);

    /// <summary>
    /// Convert meters to feet
    /// </summary>
    decimal ConvertMeterToFeet(decimal meters);

    /// <summary>
    /// Convert feet to meters
    /// </summary>
    decimal ConvertFeetToMeter(decimal feet);
}
