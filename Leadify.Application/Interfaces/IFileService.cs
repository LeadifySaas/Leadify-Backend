using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Leadify.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> GuardarArchivo(IFormFile archivo, string subCarpeta);
        Task EliminarArchivo(string rutaRelativa);
    }
}
