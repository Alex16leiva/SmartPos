using Dominio.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.Finanzas
{
    public class FormasPago : Entity
    {
        public int FormaPagoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int TipoPago { get; set; }
        public int OrdenMostrar { get; set; }

       

        [NotMapped]
        public decimal Cobro { get; set; }

        [NotMapped]
        public decimal MostrarCobro { get; set; }
    }
}
