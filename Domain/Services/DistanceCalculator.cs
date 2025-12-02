namespace Domain.Services;

/// <summary>
/// Calcola la distanza tra due coordinate geografiche usando la formula di Haversine
/// </summary>
public static class DistanceCalculator
{
    private const double EarthRadiusKm = 6371.0; // Raggio terrestre in km

    /// <summary>
    /// Calcola la distanza in km tra due punti geografici
    /// </summary>
    /// <param name="lat1">Latitudine punto 1 (gradi)</param>
    /// <param name="lon1">Longitudine punto 1 (gradi)</param>
    /// <param name="lat2">Latitudine punto 2 (gradi)</param>
    /// <param name="lon2">Longitudine punto 2 (gradi)</param>
    /// <returns>Distanza in chilometri</returns>
    public static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        // Converti gradi in radianti
        var lat1Rad = DegreesToRadians(lat1);
        var lon1Rad = DegreesToRadians(lon1);
        var lat2Rad = DegreesToRadians(lat2);
        var lon2Rad = DegreesToRadians(lon2);

        // Formula di Haversine
        var dLat = lat2Rad - lat1Rad;
        var dLon = lon2Rad - lon1Rad;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = EarthRadiusKm * c;

        return Math.Round(distance, 2); // Arrotonda a 2 decimali
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}