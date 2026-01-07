using Dominio.Core;

namespace Dominio.Context.Entidades
{
    public class Vendedor : Entity
    {
        public int VendedorId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
    }
}
