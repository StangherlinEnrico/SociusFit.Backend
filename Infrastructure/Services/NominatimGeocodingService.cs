using Domain.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

/// <summary>
/// Implementazione del servizio di geocoding usando OpenStreetMap Nominatim
/// </summary>
public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NominatimGeocodingService> _logger;
    private const string NominatimApiUrl = "https://nominatim.openstreetmap.org/search";

    public NominatimGeocodingService(
        HttpClient httpClient,
        ILogger<NominatimGeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Configurazione required da Nominatim
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SociusFit/1.0 (contact@sociusfit.com)");
    }

    public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Estrai il nome del comune dal formato "Comune (Regione)"
            var municipalityName = ExtractMunicipalityName(cityName);

            _logger.LogInformation("Geocoding city: {CityName} (extracted: {Municipality})", cityName, municipalityName);

            // Costruisci URL query
            var queryUrl = $"{NominatimApiUrl}?q={Uri.EscapeDataString(municipalityName)},Italy&format=json&limit=1";

            // Chiama Nominatim API
            var response = await _httpClient.GetAsync(queryUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Nominatim API returned status code: {StatusCode} for city: {CityName}",
                    response.StatusCode, cityName);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(content);

            if (results == null || results.Count == 0)
            {
                _logger.LogWarning("No geocoding results found for city: {CityName}", cityName);
                return null;
            }

            var firstResult = results[0];

            if (!double.TryParse(firstResult.Lat, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var latitude) ||
                !double.TryParse(firstResult.Lon, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var longitude))
            {
                _logger.LogWarning("Failed to parse coordinates from Nominatim response for city: {CityName}", cityName);
                return null;
            }

            _logger.LogInformation("Successfully geocoded {CityName} to ({Latitude}, {Longitude})",
                cityName, latitude, longitude);

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding city: {CityName}", cityName);
            return null;
        }
    }

    /// <summary>
    /// Estrae il nome del comune dal formato "Comune (Regione)"
    /// </summary>
    private string ExtractMunicipalityName(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return cityName;

        // Rimuovi la parte "(Regione)" se presente
        var openParenIndex = cityName.IndexOf('(');
        if (openParenIndex > 0)
        {
            return cityName.Substring(0, openParenIndex).Trim();
        }

        return cityName.Trim();
    }

    // DTO per deserializzare risposta Nominatim
    private class NominatimResult
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = string.Empty;

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = string.Empty;

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
    }
}