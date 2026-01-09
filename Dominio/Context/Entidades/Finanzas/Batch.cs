using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.Finanzas
{
    public class Batch : Entity
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

        [ForeignKey("CajaId")]
        public virtual Caja Caja { get; set; }

        public virtual ICollection<Diario> Diario { get; set; }

        public virtual ICollection<FacturaEncabezado> FacturaEncabezado { get; set; }
        public virtual ICollection<CuentasPorCobrarHistorial> CuentasPorCobrarHistorial { get; set; }

        public void ActualizarEstadoNuevo()
        {
            Estado = "Nuevo";
        }

        public void ActualizarEstadoCerrado()
        {
            Estado = "Cerrado";
        }

        internal void AgregarDiario(Diario diario)
        {
            if (Diario == null)
            {
                Diario = new List<Diario>();
            }

            Diario.Add(diario);
        }

        internal void AgregarFacturaEncabezado(FacturaEncabezado facturaEncabezado)
        {
            if (FacturaEncabezado == null)
            {
                FacturaEncabezado = new List<FacturaEncabezado>();
            }

            FacturaEncabezado.Add(facturaEncabezado);
        }

        internal void AgregarCuentasPorCobrarHistorial(CuentasPorCobrarHistorial cuentaPorCobrarHistorial)
        {
            if (CuentasPorCobrarHistorial == null)
            {
                CuentasPorCobrarHistorial = new List<CuentasPorCobrarHistorial>();
            }

            CuentasPorCobrarHistorial.Add(cuentaPorCobrarHistorial);
        }
    }
}
