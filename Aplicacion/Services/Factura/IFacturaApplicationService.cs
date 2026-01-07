using Aplicacion.DTOs.Factura;

namespace Aplicacion.Services.Factura
{
    public interface IFacturaApplicationService
    {
        FacturaResponse AgregarArticuloAFactura(AgregarArticuloRequest request);
    }
}
