using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.Articulos
{
    public class Articulo : Entity
    {
        [Key]
        [StringLength(50)]
        public string ArticuloId { get; set; } = string.Empty; // Código de barras o SKU

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        public string? DescripcionExtendida { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Cantidad { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Costo { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UltimoCosto { get; set; }

        // Niveles de precios
        [Column(TypeName = "decimal(18,4)")]
        public decimal Precio { get; set; }
        public decimal PrecioA { get; set; }
        public decimal PrecioB { get; set; }
        public decimal PrecioC { get; set; }
        public decimal PrecioD { get; set; }
        public decimal PrecioE { get; set; }

        // Relaciones (FKs)
        public int TipoArticuloId { get; set; }
        public int DepartamentoId { get; set; }
        public int CategoriaId { get; set; }
        public int ImpuestoId { get; set; }
        public int ProveedorId { get; set; }

        // Ofertas y Rebajas
        public bool OfertaActiva { get; set; }
        public decimal PrecioOferta { get; set; }
        public DateTime? FechaInicioOferta { get; set; }
        public DateTime? FechaFinalOferta { get; set; }

        // Control de Inventario
        public decimal PuntoReorden { get; set; }
        public decimal CantidadComprometida { get; set; }
        public string? UnidadMedida { get; set; }
        public DateTime? UltimoRecibo { get; set; }
        public DateTime? UltimaVenta { get; set; }

        // Personalización y Otros
        public string? TextoPersonalizado1 { get; set; }
        public string? TextoPersonalizado2 { get; set; }
        public string? TextoPersonalizado3 { get; set; }
        public string? ImagenRuta { get; set; }
        public string? Notas { get; set; }
        public bool Inactivo { get; set; }
        public virtual Impuesto Impuesto { get; set; }
        public virtual ICollection<FacturaDetalle> FacturaDetalle { get; set; }
        [ForeignKey("ArticuloId")]
        public virtual ICollection<InventarioMovimiento> InventarioMovimiento { get; set; }

        public void ActualizarCosto(decimal costo)
        {
            if (Costo == costo || costo <= 0)
            {
                return;
            }
            UltimoCosto = Costo;
            Costo = costo;
        }

        public decimal ObtenerPorcentajeImpuesto()
        {
            return TieneImpuesto() ? Convert.ToDecimal(Impuesto.Porcentaje / 100) : 0;
        }

        internal bool TienePromocion()
        {
            DateTime fechaActual = DateTime.Now;

            return OfertaActiva && (FechaInicioOferta <= fechaActual && FechaFinalOferta >= fechaActual);
        }

        internal decimal ObtenerPrecioSinImpuesto(decimal precio)
        {
            decimal porcentajeImpuesto = 0;
            if (TieneImpuesto())
            {
                porcentajeImpuesto = ObtenerPorcentajeImpuesto();
            }
            return (precio / (porcentajeImpuesto + 1));
        }

        public bool HayCantidadSuficiente(decimal cantidadRequerida)
        {
            return cantidadRequerida <= Cantidad;
        }

        public bool TieneImpuesto()
        {
            return Impuesto != null;
        }

        public void AjusteDeAumento(decimal cantida)
        {
            Cantidad += cantida;
            UltimoRecibo = DateTime.Now;
        }

        public bool RebajarCantidad(decimal cantidadMovimiento)
        {
            if (HayCantidadSuficiente(cantidadMovimiento))
            {
                Cantidad -= cantidadMovimiento; 
                return true; 
            }
            return false; 
        }

       
    }
}
