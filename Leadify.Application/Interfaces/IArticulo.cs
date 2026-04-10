using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.Interfaces
{
    public interface IArticulo
    {
        Task<string?> LoginAsync(string email, string password);
    }
}
