namespace Aplicacion.DTOs.Factura
{
    public class FacturaResponse : ResponseBase
    {
        public FacturaEncabezadoDTO FacturaEncabezado { get; set; }
        public List<FacturaDetalleDTO> FacturaDetalle { get; set; }
    }
}
