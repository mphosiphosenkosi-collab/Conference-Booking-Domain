using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ConferenceRoomBooking.Services.Exceptions;

namespace ConferenceRoomBooking.Services.Services
{
    public class JsonDataService : IDataService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        public async Task SaveToFileAsync<T>(string filePath, T data)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            
            try
            {
                var json = JsonSerializer.Serialize(data, _options);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new DataAccessException($"Failed to save data to {filePath}", ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Unexpected error saving to {filePath}", ex);
            }
        }
        
        public async Task<T?> LoadFromFileAsync<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
            
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<T>(json, _options);
            }
            catch (JsonException ex)
            {
                throw new DataAccessException($"Invalid JSON format in {filePath}", ex);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new DataAccessException($"Failed to load data from {filePath}", ex);
            }
        }
    }
}
