namespace Aplicacion.DTOs.Articulos
{
    public class InventarioMovimientoDTO : ResponseBase
    {
        public int InventarioMovimientoId { get; set; }
        public string ArticuloId { get; set; } = string.Empty;
        public decimal CantidadAnterior { get; set; }
        public decimal CantidadMovimiento { get; set; } // + para entrada, - para salida
        public decimal CantidadNueva { get; set; }
        public decimal CostoUnitario { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty; // Ejemplo: "Entrada", "Salida", "Ajuste"
        public string? Referencia { get; set; } // Puede ser un número de factura, orden de compra, etc.
        public string? Notas { get; set; }
    }
}
