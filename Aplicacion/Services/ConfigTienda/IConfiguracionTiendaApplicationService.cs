using Aplicacion.DTOs.ConfiTienda;

namespace Aplicacion.Services.ConfiTienda
{
    public interface IConfiguracionTiendaApplicationService
    {
        public ConfiguracionTiendaDTO ObtenerConfiguracionTienda(ConfiguracionTiendaRequest request);
        public string GuardarCambiosConfiguracionTienda(ConfiguracionTiendaRequest request);
    }
}
