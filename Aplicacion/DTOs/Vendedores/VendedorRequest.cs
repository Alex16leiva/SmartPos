namespace Aplicacion.DTOs.Vendedores
{
    public class ObtenerVendedor : RequestBase
    {
        public VendedorDTO Vendedor { get; set; }
    }

    public class CrearVenderor : RequestBase
    {
        public VendedorDTO Vendedor { get; set; }
    }

    public class ActualizarVenderor : RequestBase
    {
        public VendedorDTO Vendedor { get; set; }
    }
}
