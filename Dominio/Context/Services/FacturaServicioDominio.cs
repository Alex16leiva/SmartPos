using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;

namespace Dominio.Context.Services
{
    public class FacturaServicioDominio : IFacturaServicioDominio
    {
        public List<FacturaDetalle> ObtenerFacturaDetalle(Articulo articulo, List<FacturaDetalle> facturasDetalle,
                Vendedor vendedor, out string mensajeValidacion)
        {
            IEnumerable<FacturaDetalle> articulosAgregadorEnFactura =
                facturasDetalle.Where(r => r.ArticuloId == articulo.ArticuloId);

            decimal cantidadArticulos = 1;

            if (articulosAgregadorEnFactura.Any())
            {
                cantidadArticulos += articulosAgregadorEnFactura.Sum(r => r.Cantidad);
            }

            if (!articulo.HayCantidadSuficiente(cantidadArticulos))
            {
                mensajeValidacion = $"La cantidad {cantidadArticulos} de articulos Es mayor al inventario";
                return facturasDetalle;
            }

            decimal precio = articulo.Precio;
            DateTime fechaActual = DateTime.Now;
            bool mostrarNegrita = false;

            if (articulo.TienePromocion())
            {
                precio = articulo.PrecioOferta;
                mostrarNegrita = true;
            }

            decimal precioSinImpuesto = articulo.ObtenerPrecioSinImpuesto(precio);
            decimal impuestoArticulo = Math.Abs(precioSinImpuesto - precio);

            FacturaDetalle nuevoFacturaDetalle = new FacturaDetalle
            {
                ArticuloId = articulo.ArticuloId,
                DescripcionExtendida = articulo.DescripcionExtendida,
                Descripcion = articulo.Descripcion,
                Precio = precio,
                Cantidad = 1,
                CantidadOriginal = 1,
                CantidadArticulos = articulo.Cantidad,
                DescripcionImpuesto = articulo.Impuesto?.Descripcion,
                PorcentajeImpuesto = articulo.ObtenerPorcentajeImpuesto(),
                Costo = articulo.Costo,
                PrecioFinal = articulo.Precio,
                PagaImpuesto = articulo.TieneImpuesto(),
                MostrarNegrita = mostrarNegrita,
                PrecioSinImpuesto = precioSinImpuesto,
                VendedorId = vendedor.VendedorId,
                NombreVendedor = vendedor.Nombre,
                ArticuloImpuesto = impuestoArticulo,
                Articulo = articulo
            };

            facturasDetalle.Add(nuevoFacturaDetalle);

            CalcularTotales(facturasDetalle);

            mensajeValidacion = string.Empty;
            return facturasDetalle;
        }

        public List<FacturaDetalle> CalcularTotales(List<FacturaDetalle> facturasDetalle)
        {
            foreach (var item in facturasDetalle)
            {
                item.PrecioSinImpuesto = item.Articulo.ObtenerPrecioSinImpuesto(item.Precio);
                item.PorcentajeImpuesto = item.Articulo.ObtenerPorcentajeImpuesto();
                decimal descuento = Math.Abs(item.Precio - item.PrecioFinal);
                item.Descuento = Math.Round((descuento * item.Cantidad), 2);
                item.SubTotal = Math.Round((item.Cantidad * item.PrecioSinImpuesto), 2);
                item.Impuesto = Math.Round((item.Cantidad * item.PrecioSinImpuesto) * item.PorcentajeImpuesto, 2);
                item.Total = Math.Round((item.Cantidad * item.PrecioSinImpuesto) + item.Impuesto, 2);
                item.MostrarNegrita = descuento != 0;
            }
            return facturasDetalle;
        }

        public List<FacturaDetalle> CalcularFacturasDetalle(List<FacturaDetalle> facturasDetalleEntidad, 
            IEnumerable<Articulo> articulosEntidad, IEnumerable<Vendedor> vendedoresEntidad)
        {
            List<FacturaDetalle> facturasDetalle = new List<FacturaDetalle>();
            foreach (var item in facturasDetalleEntidad)
            {
                Articulo articulo = articulosEntidad.FirstOrDefault(r => r.ArticuloId == item.ArticuloId);
                Vendedor vendedor = vendedoresEntidad.FirstOrDefault(r => r.VendedorId == item.VendedorId);
                decimal cantidadArticulos = facturasDetalle.Where(r => r.ArticuloId == item.ArticuloId).Sum(r => r.Cantidad);
                cantidadArticulos += item.Cantidad;
                if (!articulo.HayCantidadSuficiente(cantidadArticulos))
                {
                    item.Cantidad = item.CantidadOriginal;
                    if (!articulo.HayCantidadSuficiente(item.Cantidad))
                    {
                        continue;
                    }
                }

                if (vendedor == null)
                {
                    item.VendedorId = 0;
                    item.NombreVendedor = string.Empty;
                }
                else
                {
                    item.VendedorId = vendedor.VendedorId;
                    item.NombreVendedor = vendedor.Nombre;
                }
                item.Articulo = articulo;

                facturasDetalle.Add(item);
            }

            CalcularTotales(facturasDetalle);

            return facturasDetalle;
        }
    }
}
