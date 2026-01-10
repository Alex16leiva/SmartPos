namespace Aplicacion.DTOs.ConfiTienda
{
    public class ConfiguracionTiendaDTO : ResponseBase
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Ciudad { get; set; }
        public string RTN { get; set; }
        public string Direccion1 { get; set; }
        public string Direccion2 { get; set; }
        public string CodigoZip { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string CorreoElectronico { get; set; }
        public string Personalizado1 { get; set; }
        public string Personalizado2 { get; set; }
        public string Personalizado3 { get; set; }
        public string Personalizado4 { get; set; }
    }
}
