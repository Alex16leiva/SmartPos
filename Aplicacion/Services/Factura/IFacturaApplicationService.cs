using Aplicacion.DTOs.Factura;

namespace Aplicacion.Services.Factura
{
    public interface IFacturaApplicationService
    {
        FacturaResponse AgregarArticuloAFactura(FacturaRequest request);
        FacturaResponse CalcularFacturaDetalle(FacturaRequest request);
        Task<FacturaResponse> CrearFactura(FacturaRequest request);
    }
}
