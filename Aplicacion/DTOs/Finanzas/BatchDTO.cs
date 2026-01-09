namespace Aplicacion.DTOs.Finanzas
{
    public class BatchDTO : ResponseBase
    {
        public string BatchId { get; set; }
        public int CajaId { get; set; }
        public string Estado { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal CierreTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal VentasCredito { get; set; }
        public decimal PagoACuenta { get; set; }
        public decimal Devoluciones { get; set; }
        public int CantidadClientes { get; set; }
        public decimal TotalFormaDePago { get; set; }
        public decimal Comision { get; set; }
        public decimal CambioTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal CostoTotal { get; set; }
        public string ModificadoPor { get; set; }

        public BatchDTO BatchImpresion { get; set; }
    }
}