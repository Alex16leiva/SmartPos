namespace Aplicacion.DTOs.Factura
{
    public class FacturaEncabezadoDTO : ResponseBase
    {
        public string FacturaId { get; set; }
        public string BatchId { get; set; }
        public int CajaId { get; set; }
        public string ClienteId { get; set; }
        public int CajeroId { get; set; }
        public decimal Total { get; set; }
        public decimal Impuesto { get; set; }
        public string Comentario { get; set; }
        public string NumeroReferencia { get; set; }
        public string CampoPersonalizado1 { get; set; }
        public string CampoPersonalizado2 { get; set; }
        public string LLamadaId { get; set; }
        public string LlamadaTipo { get; set; }
        public string CAI { get; set; }
        public string Correlativo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<FacturaDetalleDTO> FacturaDetalle { get; set; }
    }
}
