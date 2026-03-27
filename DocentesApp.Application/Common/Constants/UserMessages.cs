using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.Common.Constants
{
    public class UserMessages
    {        
        public const string ErrorGet = "Error ejecutando GET en {Controller}";
        public const string ErrorPost = "Error ejecutando POST en {Controller} ";
        public const string ErrorPut = "Error ejecutando PUT en {Controller} ";
        public const string ErrorDelete = "Error ejecutando DELETE en {Controller} ";
        
        public const string ErrorIdNotFound = "No se encontro el recurso con Id {Id}. ";

        public const string Error500 = "Ocurrió un error al procesar la solicitud. Intente nuevamente más tarde.";

        public const string BadRequest = "Los datos enviados no son válidos.";
        public const string NotFound = "El recurso solicitado no fue encontrado.";
        public const string Conflict = "No se puede realizar la operación debido a un conflicto con el estado actual.";
        public const string Unauthorized = "Debe iniciar sesión para realizar esta acción.";
        public const string Forbidden = "No tiene permisos para realizar esta acción.";
        public const string Unhandled = "Error no controlado en el procesamiento de la solicitud.";


    }
}
