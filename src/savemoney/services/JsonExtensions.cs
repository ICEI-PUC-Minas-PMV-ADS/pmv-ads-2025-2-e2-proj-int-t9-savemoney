using System.Text.Json;

namespace savemoney.Services
{
    // Métodos de extensão para facilitar o tratamento de campos ausentes
    public static class JsonExtensions
    {
        // ATUALIZADO: Adicionado '?' para indicar que o retorno e o padrão podem ser nulos (string?).
        public static string? GetPropertyOrDefault(this JsonElement element, string property, string? defaultValue)
        {
            if (element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String)
                // GetString() já retorna string? por padrão.
                return value.GetString();
            return defaultValue;
        }

        public static int GetPropertyOrDefault(this JsonElement element, string property, int defaultValue)
        {
            if (element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
                return intValue;
            return defaultValue;
        }
    }
}