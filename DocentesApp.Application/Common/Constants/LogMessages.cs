using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.Common.Constants
{
    public class LogMessages
    {
        public const string ErrorLogUnhandled = "Error no controlado en el procesamiento de la solicitud.";
        public const string ErrorLogBadRequest = "Error al procesar la solicitud: datos inválidos.";
        public const string ErrorLogNotFound = "Recurso no encontrado.";
        public const string ErrorLogConflict = "Conflicto detectado al procesar la solicitud.";
        public const string ErrorLogUnauthorized = "Intento de acceso no autorizado.";
        public const string ErrorLogForbidden = "Acceso denegado por falta de permisos.";
    }
}
