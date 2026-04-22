using Leadify.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Leadify.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        // Definimos la carpeta raíz de adjuntos dentro de wwwroot
        private const string RootFolder = "adjuntos";

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> GuardarArchivo(IFormFile archivo, string subCarpeta)
        {
            
            string baseRuta = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

           
            if (!Directory.Exists(baseRuta)) Directory.CreateDirectory(baseRuta);

           
            string carpetaDestino = Path.Combine(baseRuta, RootFolder, subCarpeta);

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

           
            string extension = Path.GetExtension(archivo.FileName);
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

       
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

           
            return $"/{RootFolder}/{subCarpeta}/{nombreArchivo}".Replace("\\", "/");
        }

        public async Task EliminarArchivo(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa)) return;

            // Limpiamos la ruta relativa 
            string rutaNormalizada = rutaRelativa.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());

            // Construimos la ruta completa partiendo de wwwroot
            string fullPath = Path.Combine(_env.WebRootPath, rutaNormalizada);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }
    }
}