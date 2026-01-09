using Dominio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Context.Entidades.Finanzas
{
    public class TipoCuenta : Entity
    {
        public int TipoCuentaId { get; set; }
        public string Codigo { get; set; }
        public bool EsCredito { get; set; }
        public string Descripcion { get; set; }
        public int FechaDeVencimiento { get; set; }
        public int PeriodoDeGracia { get; set; }
        public decimal CargoFinancieroMinimo { get; set; }
        public decimal TasaDeInteresAnual { get; set; }
        public bool AplicarCargosFinancierosEnCargosFinancieros { get; set; }
        public decimal PagoMinimo { get; set; }
        public virtual ICollection<Cliente> Cliente { get; set; }
    }
}
