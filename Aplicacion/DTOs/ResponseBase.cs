using Dominio.Core.Extensions;

namespace Aplicacion.DTOs
{
    public abstract class ResponseBase
    {
        public string? Message { get; set; }
        public string? ValidationErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public DateTime? FechaTransaccion { get; set; }
        public string? ModificadoPor { get; set; }
        public bool HasValidationMessage() => Message.HasValue();

        public bool HasValidationErrorMessage() => ValidationErrorMessage.HasValue();

        public bool HasAnyMessage()
        {
            return HasValidationErrorMessage() || HasValidationMessage();
        }

        public void AppendValidationErrorMessage(string message)
        {
            if (HasValidationErrorMessage())
            {
                ValidationErrorMessage = $"{ValidationErrorMessage}, {message}";
                return;
            }
            ValidationErrorMessage = message;
        }

        public string Getmessage()
        {
            if (HasValidationErrorMessage()) return ValidationErrorMessage!;
            if (HasValidationMessage()) return Message!;
            return string.Empty;
        }
    }
}
