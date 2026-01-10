using Aplicacion.DTOs;

namespace ServicioAplicacion.DTOs.TipoCuenta
{
    public class TipoCuentaRequest : RequestBase
    {
        public TipoCuentaDTO TipoCuentas { get; set; }
    }
}
