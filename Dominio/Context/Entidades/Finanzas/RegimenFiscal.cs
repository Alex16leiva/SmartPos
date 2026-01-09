using Dominio.Core;
using System.Text;

namespace Dominio.Context.Entidades.Finanzas
{
    public class RegimenFiscal : Entity
    {
        public int Id { get; set; }
        public string Sucursal { get; set; }
        public string CAI { get; set; }
        public string Desde { get; set; }
        public string Hasta { get; set; }
        public int CorrelativoActual { get; set; }
        public int CantidadCaracteres { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaLimiteEmision { get; set; }

        public void DesactivarCAI()
        {
            Activo = false;
        }

        public string ObtenerCorrelativoNuevo()
        {
            int cantidadCeros = 0;
            CorrelativoActual++;
            cantidadCeros = (8 - CorrelativoActual.ToString().Length) * 1;
            var resultado = Desde.Substring(0, Desde.Length - CantidadCaracteres);
            return $"{resultado}{new StringBuilder(cantidadCeros).Insert(0, "0", cantidadCeros)}{CorrelativoActual}";

        }
    }
}
