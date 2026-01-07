namespace Aplicacion.DTOs.Factura
{
    public class FacturaDetalleDTO : ResponseBase
    {
        public int Id { get; set; }
        public string FacturaId { get; set; }
        public string ArticuloId { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioFinal { get; set; }
        public decimal Costo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Impuesto { get; set; }
        public int VendedorId { get; set; }
        public decimal Comision { get; set; }
        public string Comentario { get; set; }
        public decimal Descuento { get; set; }
        public decimal CantidadOriginal { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionExtendida { get; set; }
        public decimal CantidadArticulos { get; set; }
        public string DescripcionImpuesto { get; set; }
        public decimal PorcentajeImpuesto { get; set; }
        public bool PagaImpuesto { get; set; }
        public bool MostrarNegrita { get; set; }
        public decimal PrecioSinImpuesto { get; set; }
        public string NombreVendedor { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }

    }
}
