using Aplicacion.DTOs;

namespace ServicioAplicacion.DTOs.TipoCuenta
{
    public class TipoCuentaDTO : ResponseBase
    {
        public int TipoCuentaId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool EsCredito { get; set; }
        public int FechaDeVencimiento { get; set; }
        public int PeriodoDeGracia { get; set; }
        public decimal CargoFinancieroMinimo { get; set; }
        public decimal TasaDeInteresAnual { get; set; }
        public bool AplicarCargosFinancierosEnCargosFinancieros { get; set; }
        public decimal PagoMinimo { get; set; }

    }
}
