using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;
using Dominio.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.FacturaAgg
{
    public class FacturaEncabezado : Entity
    {
        public string FacturaId { get; set; }
        public string TipoFactura { get; set; }
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
        public decimal Saldo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string EstadoFactura { get; set; }
        public virtual ICollection<FacturaDetalle> FacturaDetalle { get; set; }
        public virtual ICollection<FormaPagoDetalle> FormaPagoDetalle { get; set; }
        public virtual ICollection<CuentasPorCobrar> CuentaPorCobrar { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual ICollection<Diario> Diario { get; set; }
        [NotMapped]
        public string Desde { get; set; }

        [NotMapped]
        public string Hasta { get; set; }

        [NotMapped]
        public DateTime FechaLimiteEmision { get; set; }

        [NotMapped]
        public decimal SubTotal { get; set; }

        [NotMapped]
        public decimal Cambio { get; set; }

        [NotMapped]
        public decimal Descuento { get; set; }

        [NotMapped]
        public bool EsDevolucion { get; set; }
        internal void AgregarFacturaDetalle(List<FacturaDetalle> facturaDetalle)
        {
            if (FacturaDetalle.IsNull())
            {
                FacturaDetalle = [];
            }

            FacturaDetalle = facturaDetalle;
        }

        public void AgregarFormaPagoDetalle(List<FormaPagoDetalle> formaPagoDetalle)
        {
            if (FormaPagoDetalle.IsNull())
            {
                FormaPagoDetalle = [];
            }

            FormaPagoDetalle = formaPagoDetalle;
        }

        public void AsignarlDescripcionFacturaDetalle()
        {
            if (FacturaDetalle.IsNotNull())
            {
                FacturaDetalle.ToList().ForEach(item =>
                    item.Descripcion = (item.Articulo.IsNull()) ? string.Empty : item.Articulo.Descripcion);
            }
        }

        internal void AgregarCuentaPorCobrar(CuentasPorCobrar cuentaPorCobrar)
        {
            if (CuentaPorCobrar.IsNull())
            {
                CuentaPorCobrar = [];
            }

            CuentaPorCobrar.Add(cuentaPorCobrar);
        }

        public string ObtenerNombreCliente()
        {
            var nombreCliente = string.Empty;
            if (CampoPersonalizado1.HasValue())
            {
                nombreCliente = CampoPersonalizado1;
            }
            else
            {
                if (Cliente.IsNotNull())
                {
                    nombreCliente = Cliente.Nombre.ValueOrEmpty();
                }
            }

            return nombreCliente;
        }
    }
}
