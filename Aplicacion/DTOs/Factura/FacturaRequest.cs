using Aplicacion.DTOs.FormasPagos;
using Aplicacion.DTOs.Vendedores;

namespace Aplicacion.DTOs.Factura
{
    public class FacturaRequest : RequestBase
    {
        public string ArticuloId { get; set; }
        public VendedorDTO Vendedor { get; set; }
        public List<FacturaDetalleDTO> FacturaDetalle { get; set; }
        public FacturaEncabezadoDTO FacturaEncabezado { get; set; }
        public FacturaDetalleDTO FacturaDetalleSeleccionado { get; set; }
        public List<FormasPagoDTO> FormasPagos { get; set; }
    }
}
