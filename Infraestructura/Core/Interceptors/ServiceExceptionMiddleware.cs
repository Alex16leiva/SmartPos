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
            invocation.Proceed();

            var method = invocation.MethodInvocationTarget;
            bool isAsyncTask = typeof(Task).IsAssignableFrom(method.ReturnType);

            if (isAsyncTask)
            {
                // Usamos dynamic para que invoque la versión genérica correcta en tiempo de ejecución
                invocation.ReturnValue = HandleAsyncWithResult((dynamic)invocation.ReturnValue, invocation);
            }
        }

        // Versión para métodos que devuelven Task<T> (¡Esta es la que arregla tu error!)
        private async Task<T> HandleAsyncWithResult<T>(Task<T> task, IInvocation invocation)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(invocation, ex);
                throw;
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
            var baseEx = ex.GetBaseException();

            // 1.Obtenemos el nombre de la clase(Modulo) y el método automáticamente
            string modulo = invocation.TargetType.Name;
            string metodo = invocation.Method.Name;


            // 2. SELLO: Marcamos la excepción para que el Dispatcher no la repita
            if (!baseEx.Data.Contains("Logged")) baseEx.Data.Add("Logged", true);
            if (!ex.Data.Contains("Logged")) ex.Data.Add("Logged", true);

            // 3. Guardamos en SQL mediante el servicio
            await _logService.LogErrorAsync(modulo, metodo, ex, "System_Auto");
        }
    }
}