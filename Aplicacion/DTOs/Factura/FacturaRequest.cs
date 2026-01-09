using Aplicacion.DTOs.Vendedores;

namespace Aplicacion.DTOs.Factura
{
    public class FacturaRequest : RequestBase
    {
        public string ArticuloId { get; set; }
        public VendedorDTO Vendedor { get; set; }
        public List<FacturaDetalleDTO> FacturasDetalle { get; set; }
        public FacturaDetalleDTO FacturaDetalleSeleccionado { get; set; }
    }
}
