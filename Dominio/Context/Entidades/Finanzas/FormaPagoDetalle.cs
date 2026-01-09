using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core;

namespace Dominio.Context.Entidades.Finanzas
{
    public class FormaPagoDetalle : Entity
    {
        public int Id { get; set; }
        public string BatchId { get; set; }
        public string FacturaId { get; set; }
        public int FormaPagoId { get; set; }
        public string PagoId { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoExtranjero { get; set; }

        public virtual FacturaEncabezado FacturaEncabezado { get; set; }
    }
}
