using System;

namespace Infraestructura.Core.Logging
{
    public interface ILogService
    {
        Task LogErrorAsync(string modulo, string metodo, System.Exception ex, string user);
    }
}
