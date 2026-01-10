using Aplicacion.DTOs;
using Aplicacion.DTOs.Clientes;
using ServicioAplicacion.DTOs.TipoCuenta;

namespace Aplicacion.Services.ClienteServices
{
    public interface IClienteApplicationService
    {
        SearchResult<ClienteDTO> ObtenerCliente(ClienteRequest request);

        ClienteDTO CrearCliente(ClienteRequest request);

        ClienteDTO ActualizarCliente(ClienteRequest request);

        List<TipoCuentaDTO> ObtenerTipoCuentas(TipoCuentaRequest request);
    }
}
