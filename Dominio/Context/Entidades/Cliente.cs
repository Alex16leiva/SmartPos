using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;

namespace Dominio.Context.Entidades
{
    public class Cliente : Entity
    {
        public int Id { get; set; }
        public string NumeroCuenta { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Compania { get; set; }
        public string Direccion { get; set; }
        public string Direccion2 { get; set; }
        public string Pais { get; set; }
        public string Departamento { get; set; }
        public string Ciudad { get; set; }
        public string Zip { get; set; }
        public string NumeroTelefono1 { get; set; }
        public string NumeroTelefono2 { get; set; }
        public string NumeroFax { get; set; }
        public string CorreoElectronico { get; set; }
        public int TipoCuentaID { get; set; }
        public DateTime AperturaCuenta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoCuenta { get; set; }
        public bool Empleado { get; set; }
        public int NivelPrecio { get; set; }
        public decimal DescuentoActual { get; set; }
        public string FotoRuta { get; set; }
        public DateTime UltimaVisita { get; set; }
        public int TotalVisitas { get; set; }
        public decimal VentasTotales { get; set; }
        public decimal AhorrosTotales { get; set; }
        public string Notas { get; set; }
        public int EnvioPrimarioId { get; set; }
        public DateTime FechaPersonalizada1 { get; set; }
        public DateTime FechaPersonalizada2 { get; set; }
        public DateTime FechaPersonalizada3 { get; set; }
        public DateTime FechaPersonalizada4 { get; set; }
        public DateTime FechaPersonalizada5 { get; set; }
        public decimal NumeroPersonalizado1 { get; set; }
        public decimal NumeroPersonalizado2 { get; set; }
        public decimal NumeroPersonalizado3 { get; set; }
        public decimal NumeroPersonalizado4 { get; set; }
        public decimal NumeroPersonalizado5 { get; set; }
        public string TextoPersonalizado1 { get; set; }
        public string TextoPersonalizado2 { get; set; }
        public string TextoPersonalizado3 { get; set; }
        public string TextoPersonalizado4 { get; set; }
        public string TextoPersonalizado5 { get; set; }

        public virtual ICollection<FacturaEncabezado> FacturaEncabezado { get; set; }
        public virtual TipoCuenta TipoCuenta { get; set; }

        internal void ActualizacionFacturacion(decimal ventaTotal, decimal descuento)
        {
            TotalVisitas++;
            UltimaVisita = DateTime.Now;
            VentasTotales += ventaTotal;
            AhorrosTotales += descuento;
        }

        public bool TieneCredito()
        {
            return TipoCuenta != null ? TipoCuenta.EsCredito : false;
        }
    }
}
