using Castle.DynamicProxy;
using Infraestructura.Core.Logging;

namespace Infraestructura.Interceptors
{
    public class ServiceExceptionMiddleware : IInterceptor
    {
        private readonly ILogService _logService;

        public ServiceExceptionMiddleware(ILogService logService)
        {
            _logService = logService;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                // Ejecuta el método original del servicio
                invocation.Proceed();

                // Manejo de métodos Asíncronos (Task)
                if (invocation.ReturnValue is Task task)
                {
                    invocation.ReturnValue = HandleAsync(task, invocation);
                }
            }
            catch (Exception ex)
            {
                // Manejo de métodos Síncronos
                Task.Run(() => RegistrarErrorAsync(invocation, ex)).Wait();
                throw; // Re-lanzamos para que la UI sepa que algo falló
            }
        }

        private async Task HandleAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(invocation, ex);
                throw; // Re-lanzamos la excepción
            }
        }

        private async Task RegistrarErrorAsync(IInvocation invocation, Exception ex)
        {
            // Extraemos los nombres de forma automática mediante reflexión
            string modulo = invocation.TargetType.Name;
            string metodo = invocation.Method.Name;

            // Aquí puedes incluso capturar los parámetros si quieres ser más pro
            // string parametros = string.Join(", ", invocation.Arguments);

            await _logService.LogErrorAsync(
                modulo,
                metodo,
                ex,
                "System_Middleware_Auto"
            );
        }
    }
}