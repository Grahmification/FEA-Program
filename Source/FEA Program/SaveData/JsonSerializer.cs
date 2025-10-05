using System.IO;
using System.Text.Json;

namespace FEA_Program.SaveData
{
    /// <summary>
    /// A static helper class for serializing and deserializing arbitrary C# objects 
    /// to and from JSON files using System.Text.Json.
    /// </summary>
    public static class JsonSerializer
    {
        // Configuration for JSON serialization (optional, but highly recommended)
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Make the JSON file human-readable
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase for JSON properties (standard web practice)
            AllowTrailingCommas = true, // Allow trailing commas in case of manual editing
        };

        /// <summary>
        /// Serializes an object of type T to a JSON file asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="data">The object instance to be serialized.</param>
        /// <param name="filePath">The full path to the file where the JSON should be saved.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public static async Task SerializeToJsonFile<T>(T data, string filePath) where T : class
        {
            if (data == null)
            {
                Console.WriteLine("Error: Data object cannot be null.");
                return;
            }

            try
            {
                // Use the file stream for efficient writing
                using (FileStream createStream = File.Create(filePath))
                {
                    // Serialize the object asynchronously to the stream
                    await System.Text.Json.JsonSerializer.SerializeAsync(createStream, data, DefaultOptions);
                }
                Console.WriteLine($"Serialization successful. Data saved to: {filePath}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO Error saving file: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Serialization Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during serialization: {ex.Message}");
            }
        }

        /// <summary>
        /// Deserializes a JSON file content into an object of type T asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="filePath">The full path to the JSON file to load.</param>
        /// <returns>The deserialized object of type T, or null if loading fails.</returns>
        public static async Task<T?> DeserializeFromJsonFile<T>(string filePath) where T : class
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: File not found at path: {filePath}");
                    return null;
                }

                // Use the file stream for efficient reading
                using (FileStream openStream = File.OpenRead(filePath))
                {
                    // Deserialize the stream content asynchronously
                    T data = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(openStream, DefaultOptions);
                    Console.WriteLine($"Deserialization successful. Data loaded from: {filePath}");
                    return data;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File not found: {filePath}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error: The file content is invalid JSON or does not match the class structure. Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during deserialization: {ex.Message}");
                return null;
            }
        }
    }
}
