namespace Aplicacion.DTOs.Comunes
{
    public class VendedorDTO : ResponseBase
    {
        public int VendedorId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
    }
}
