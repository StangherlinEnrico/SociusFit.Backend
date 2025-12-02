namespace Domain.Services;

/// <summary>
/// Servizio per ottenere coordinate geografiche da un indirizzo
/// </summary>
public interface IGeocodingService
{
    /// <summary>
    /// Ottiene le coordinate geografiche per una città/località
    /// </summary>
    /// <param name="cityName">Nome città nel formato "Comune (Regione)" es: "Milano (Lombardia)"</param>
    /// <param name="cancellationToken">Token di cancellazione</param>
    /// <returns>Tupla con Latitude e Longitude, o null se non trovato</returns>
    Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string cityName, CancellationToken cancellationToken = default);
}