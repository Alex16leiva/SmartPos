using Dominio.Context.Entidades.Articulos;
using Dominio.Core;

namespace Dominio.Context.Entidades
{
    public class Impuesto : Entity
    {
        public int ImpuestoId { get; set; }
        public string Descripcion { get; set; }
        public decimal Porcentaje { get; set; }
        public string Codigo { get; set; }

        public virtual ICollection<Articulo> Articulo { get; set; }
    }
}
