using Dominio.Context.Entidades.Articulos;
using Dominio.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.FacturaAgg
{
    public class FacturaDetalle : Entity
    {
        public int Id { get; set; }
        public string FacturaId { get; set; }
        public string ArticuloId { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioFinal { get; set; }
        public decimal Costo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Impuesto { get; set; }
        public int VendedorId { get; set; }
        public decimal Comision { get; set; }
        public string Comentario { get; set; }
        public decimal Descuento { get; set; }

        public virtual FacturaEncabezado FacturaEncabezado { get; set; }
        public virtual Articulo Articulo { get; set; }

        [NotMapped]
        public decimal CantidadOriginal { get; set; }

        [NotMapped]
        public string Descripcion { get; set; }

        [NotMapped]
        public string DescripcionExtendida { get; set; }

        [NotMapped]
        public decimal CantidadArticulos { get; set; }

        [NotMapped]
        public string DescripcionImpuesto { get; set; }

        [NotMapped]
        public decimal PorcentajeImpuesto { get; set; }

        [NotMapped]
        public bool PagaImpuesto { get; set; }

        [NotMapped]
        public bool MostrarNegrita { get; set; }

        [NotMapped]
        public decimal PrecioSinImpuesto { get; set; }

        [NotMapped]
        public string NombreVendedor { get; set; }

        [NotMapped]
        public decimal ArticuloImpuesto { get; set; }

        [NotMapped]
        public decimal SubTotal { get; set; }

        [NotMapped]
        public decimal Total { get; set; }

        public string ObtenerDescripcionArticulo()
        {
            var descripcion = string.Empty;
            if (Articulo != null && !string.IsNullOrWhiteSpace(Articulo.Descripcion))
            {
                descripcion = Articulo.Descripcion;
            }

            return descripcion;
        }

        public string ObtenerDescripcionImpuesto()
        {
            var descripcion = string.Empty;
            if (Articulo != null)
            {
                if (Articulo.Impuesto != null && !string.IsNullOrWhiteSpace(Articulo.Impuesto.Descripcion))
                {
                    descripcion = Articulo.Impuesto.Descripcion;
                }
            }

            return descripcion;
        }

        public decimal ObtenerPorcentajeImpuesto()
        {
            decimal porcentajeImpuesto = 0;
            if (Articulo != null)
            {
                if (Articulo.Impuesto != null)
                {
                    porcentajeImpuesto = Articulo.Impuesto.Porcentaje;
                }
            }
            return porcentajeImpuesto;
        }
    }
}
