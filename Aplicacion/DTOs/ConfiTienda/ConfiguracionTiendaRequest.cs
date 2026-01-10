namespace Aplicacion.DTOs.ConfiTienda
{
    public class ConfiguracionTiendaRequest : RequestBase
    {
        public bool EsAdminPos { get; set; }
        public ConfiguracionTiendaDTO ConfiguracionTienda { get; set; }
    }
}
