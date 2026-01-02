using Aplicacion.Services.ArticuloServices;
using Castle.DynamicProxy;
using Infraestructura.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace SmartPos.Comunes.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddApplicationServicesWithInterceptors(this IServiceCollection services)
        {
            // 1. Registrar la infraestructura necesaria
            services.AddSingleton<ProxyGenerator>();
            services.AddTransient<ServiceExceptionMiddleware>();

            // 2. Escaneo automático por reflexión
            var assembly = typeof(ArticuloApplicationService).Assembly;

            // Buscamos interfaces que sigan la convención "*ApplicationService"
            var servicePairs = assembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("ApplicationService"))
                .Select(interfaceType => new
                {
                    Interface = interfaceType,
                    Implementation = assembly.GetTypes()
                        .FirstOrDefault(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface)
                })
                .Where(p => p.Implementation != null);

            foreach (var pair in servicePairs)
            {
                // Registramos la clase concreta
                services.AddTransient(pair.Implementation);

                // Registramos la interfaz envuelta en el Middleware (Proxy)
                services.AddTransient(pair.Interface, sp =>
                {
                    var proxyGenerator = sp.GetRequiredService<ProxyGenerator>();
                    var actualService = sp.GetRequiredService(pair.Implementation);
                    var middleware = sp.GetRequiredService<ServiceExceptionMiddleware>();

                    return proxyGenerator.CreateInterfaceProxyWithTarget(pair.Interface, actualService, middleware);
                });
            }
        }
    }
}
