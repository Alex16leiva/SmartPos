using Aplicacion.DTOs;
using Aplicacion.DTOs.FormasPagos;

namespace Aplicacion.Services.FPagos
{
    public interface IFormasPagoApplicationService
    {
        SearchResult<FormasPagoDTO> ObtenerFormasDePagoPaginado(FormaPagoRequest request);

        List<FormasPagoDTO> ObtenerFormasDePago(FormaPagoRequest request);

        FormasPagoDTO CrearFormaPago(FormaPagoRequest request);

        FormasPagoDTO ActualizarFormaPago(FormaPagoRequest request);
    }
}
