using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class EmpresaDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string? DireccionFiscal { get; set; }
        public string? EmailFacturacion { get; set; }
        public string? CondicionIva { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateEmpresaDto
    {
        public int ClienteId { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string? DireccionFiscal { get; set; }
        public string? EmailFacturacion { get; set; }
        public string? CondicionIva { get; set; }
    }
}
