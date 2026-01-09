using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core;

namespace Dominio.Context.Entidades.Finanzas
{
    public class Diario : Entity
    {
        public int Id { get; set; }
        public string TipoTransaccionId { get; set; }

        public string Referencia { get; set; }
        public int CajaId { get; set; }
        public string BatchId { get; set; }
        public virtual Batch Batch { get; set; }

        public virtual FacturaEncabezado FacturaEncabezado { get; set; }
    }
}
