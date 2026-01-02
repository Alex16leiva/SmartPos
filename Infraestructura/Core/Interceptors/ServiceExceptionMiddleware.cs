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
                RegistrarErrorAsync(invocation, ex).GetAwaiter().GetResult();
                throw; // Re-lanzamos para que la UI sepa que algo falló
            }
        }

        private async Task HandleAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task;
            }
            catch (Exception)
            {
                // IMPORTANTE: Obtenemos la excepción real almacenada en la Task
                // Si es una AggregateException (común en Tasks), tomamos la primera
                var originalEx = task.Exception?.InnerException ?? task.Exception;

                if (originalEx != null)
                {
                    await RegistrarErrorAsync(invocation, originalEx);
                }

                throw; // Al hacer 'throw' sin variable, preservamos el stack trace original
            }
        }

        private async Task RegistrarErrorAsync(IInvocation invocation, Exception ex)
        {
            // 1.Obtenemos el nombre de la clase(Modulo) y el método automáticamente
            string modulo = invocation.TargetType.Name;
            string metodo = invocation.Method.Name;

            
            // 2. SELLO: Marcamos la excepción para que el Dispatcher no la repita
            if (!ex.Data.Contains("Logged"))
            {
                ex.Data.Add("Logged", true);
            }

            // 3. Guardamos en SQL mediante el servicio
            await _logService.LogErrorAsync(modulo, metodo, ex, "System_Auto");
        }
    }
}