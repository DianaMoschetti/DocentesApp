using System.Text.Json;
using DocentesApp.Blazor.UI.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.Blazor.UI.Helpers
{
    public static class ApiErrorHelper
    {
        /// <summary>
        /// Devuelve un mensaje legible a partir de una ApiException de NSwag.
        /// Busca en este orden:
        /// 1. ApiException&lt;ProblemDetails&gt;.Result.Detail (status declarados en el swagger: 400, 404...)
        /// 2. El body crudo (ex.Response) como JSON: errors de validación, "detail" o "message"
        ///    (status no declarados, ej. 409, llegan con el body como texto)
        /// 3. El mensaje genérico recibido por parámetro.
        /// Ojo: para status declarados ex.Response viene vacío porque el cliente lee el stream
        /// directo (ReadResponseAsString = false), por eso primero se mira Result.
        /// </summary>
        public static string GetMessage(ApiException ex, string mensajeGenerico = "Error al guardar. Intentá de nuevo.")
        {
            if (ex is ApiException<ProblemDetails> problemEx && !string.IsNullOrWhiteSpace(problemEx.Result?.Detail))
                return problemEx.Result.Detail;

            if (string.IsNullOrWhiteSpace(ex.Response))
                return mensajeGenerico;

            try
            {
                using var doc = JsonDocument.Parse(ex.Response);
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                    return mensajeGenerico;

                // ValidationProblemDetails: { "errors": { "Campo": ["msg1", "msg2"] } }
                if (TryGetProperty(root, "errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var mensajes = errors.EnumerateObject()
                        .Where(p => p.Value.ValueKind == JsonValueKind.Array)
                        .SelectMany(p => p.Value.EnumerateArray())
                        .Where(v => v.ValueKind == JsonValueKind.String)
                        .Select(v => v.GetString())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();

                    if (mensajes.Any())
                        return string.Join(" | ", mensajes);
                }

                if (TryGetProperty(root, "detail", out var detail) && detail.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(detail.GetString()))
                    return detail.GetString()!;

                if (TryGetProperty(root, "message", out var message) && message.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(message.GetString()))
                    return message.GetString()!;
            }
            catch (JsonException)
            {
                // el body no es JSON (ej. HTML de un proxy): no se muestra crudo al usuario
            }

            return mensajeGenerico;
        }

        private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = prop.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
