namespace Aplicacion.DTOs.FormasPagos
{
    public class FormasPagoDTO : ResponseBase
    {
        public int FormaPagoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int TipoPago { get; set; }
        public int OrdenMostrar { get; set; }
        public string TiposDePago { get; set; }
        public decimal Cobro { get; set; }
        public decimal MostrarCobro { get; set; }
    }
}
