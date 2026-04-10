using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string password);
    }
}
