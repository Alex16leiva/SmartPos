using Dominio.Core;

namespace Dominio.Context.Entidades.Finanzas
{
    public class CuentasPorCobrarHistorial : Entity
    {
        public int Id { get; set; }
        public int CuentaPorCobrarID { get; set; }
        public string BatchId { get; set; }
        public decimal Monto { get; set; }
        public int PagoID { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoDeHistorico { get; set; }

        public virtual CuentasPorCobrar CuentasPorCobrar { get; set; }
        public virtual Batch Batch { get; set; }
    }
}
