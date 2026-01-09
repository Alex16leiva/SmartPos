using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core;

namespace Dominio.Context.Entidades.Finanzas
{
    public class CuentasPorCobrar : Entity
    {
        public int Id { get; set; }
        public string NumeroCuenta { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaDeVencimiento { get; set; }
        public decimal CantidadOriginal { get; set; }
        public string NumeroFactura { get; set; }
        public int Tipo { get; set; }
        public decimal Balance { get; set; }

        public virtual FacturaEncabezado FacturaEncabezado { get; set; }
        public virtual ICollection<CuentasPorCobrarHistorial> CuentasPorCobrarHistorial { get; set; }
    }
}
