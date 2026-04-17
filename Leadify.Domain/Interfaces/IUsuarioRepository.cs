using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Interfaces
{
    public interface IUsuarioRepository 
    {
        Task<PagedResult<Usuario>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> CreateAsync(Usuario usuario, string password); 
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<bool> ExisteEmailAsync(string email);
    }
}
