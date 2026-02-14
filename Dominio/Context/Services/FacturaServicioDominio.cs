using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Core.Extensions;

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

            if (articulosAgregadorEnFactura.HasItems())
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

            FacturaDetalle nuevoFacturaDetalle = new()
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
            List<FacturaDetalle> facturasDetalle = [];
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

                if (vendedor.IsNull())
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

        public void CrearFactura(Batch batch, FacturaEncabezado facturaEncabezado, List<FacturaDetalle> facturaDetalle, 
            List<FormasPago> formasPago, List<Articulo> articulos, Cliente cliente, RegimenFiscal regimenFiscal)
        {
            facturaEncabezado.AgregarFacturaDetalle(facturaDetalle);

            if (facturaEncabezado.EsDevolucion)
            {
                batch.Devoluciones = (Math.Abs(batch.Devoluciones)) + (Math.Abs(facturaEncabezado.SubTotal)) * -1;
                batch.Descuento = batch.Descuento - facturaEncabezado.Descuento;
                facturaEncabezado.LlamadaTipo = "Devolucion";
                batch.TotalFormaDePago += Math.Abs(facturaEncabezado.Total * -1);
            }
            else
            {
                batch.TotalVenta += facturaEncabezado.Total;
                batch.SubTotal += facturaEncabezado.SubTotal;
                batch.CambioTotal = (Math.Abs(batch.CambioTotal) + facturaEncabezado.Cambio) * -1;
                batch.TotalFormaDePago += formasPago.Sum(r => r.Cobro);
                batch.Descuento += facturaEncabezado.Descuento;
            }

            batch.Impuesto += facturaEncabezado.Impuesto;
            batch.CantidadClientes++;
            batch.CostoTotal += facturaDetalle.Sum(r => (r.Costo * r.Cantidad));
            batch.CajaId = facturaEncabezado.CajaId;

            List<FormaPagoDetalle> formaPagoDetalle = MaterializarFormaPagoDetalle(formasPago, batch, facturaEncabezado);
            facturaEncabezado.AgregarFormaPagoDetalle(formaPagoDetalle);

            Diario diario = MaterializarDiario(facturaEncabezado);

            batch.AgregarDiario(diario);

            SincronizarStockFacturado(facturaDetalle, articulos);
            facturaEncabezado.FechaCreacion = DateTime.Now;
            batch.AgregarFacturaEncabezado(facturaEncabezado);

            if (cliente.IsNotNull())
            {
                cliente.ActualizacionFacturacion(facturaEncabezado.Total, facturaEncabezado.Descuento);
            }

            if (FacturaTieneCredito(formasPago))
            {
                var totalCredito = formasPago.Where(r => r.TipoPago == 4).Sum(r => r.Cobro);

                MaterializarCuentasPorCobrar(facturaEncabezado, totalCredito, batch, cliente.DiasLimitePagoFactura);

                batch.VentasCredito += totalCredito;

                if (!cliente.EstaDentroDelLimiteDeCredito(totalCredito)) {
                    return;
                }
                cliente.SaldoCuenta += totalCredito;
                facturaEncabezado.TipoFactura = "Credito";
                facturaEncabezado.EstadoFactura = "Pendiente";
            }
            else
            {
                facturaEncabezado.TipoFactura = "Contado";
                facturaEncabezado.EstadoFactura = "Pagada";
                facturaEncabezado.FechaVencimiento = facturaEncabezado.FechaCreacion;
            }

            facturaEncabezado.AgregarInformacionRegimenFiscal(regimenFiscal);
        }

        public bool ValidarFactura(List<FormasPago> formasPago, Cliente cliente, out string mensajeValidacion)
        {
            if (FacturaTieneCredito(formasPago))
            {
                var totalCredito = formasPago.Where(r => r.TipoPago == 4).Sum(r => r.Cobro);

                if (!cliente.TieneCredito())
                {
                    mensajeValidacion = $"El cliente {cliente.NumeroCuenta} no tiene credito";
                    return true;
                }

                if (!cliente.EstaDentroDelLimiteDeCredito(totalCredito))
                {
                    mensajeValidacion = $"El cliente {cliente.NumeroCuenta} sobre paso su limite de credito";
                    return true;
                }
            }

            mensajeValidacion = string.Empty;
            return false;
        }

        private void MaterializarCuentasPorCobrar(FacturaEncabezado facturaEncabezado, decimal totalCredito, Batch batch, int diasVencimiento)
        {
            var fechaTransaccion = DateTime.Now;
            facturaEncabezado.FechaVencimiento = fechaTransaccion.AddDays(diasVencimiento);
            var nuevoCuentaPorCobrar = new CuentasPorCobrar
            {
                Balance = totalCredito,
                CantidadOriginal = totalCredito,
                FechaDeVencimiento = fechaTransaccion.AddDays(diasVencimiento),
                NumeroCuenta = facturaEncabezado.ClienteId,
                NumeroFactura = facturaEncabezado.FacturaId,
                Tipo = 0,
                Fecha = fechaTransaccion
            };

            var nuevoCuentaPorCobrarHistorial = new CuentasPorCobrarHistorial
            {
                CuentasPorCobrar = nuevoCuentaPorCobrar,
                BatchId = batch.BatchId,
                Monto = totalCredito,
                TipoDeHistorico = 0,
                Fecha = fechaTransaccion,
                Comentario = string.Empty
            };

            batch.AgregarCuentasPorCobrarHistorial(nuevoCuentaPorCobrarHistorial);

            facturaEncabezado.AgregarCuentaPorCobrar(nuevoCuentaPorCobrar);
        }


        private bool FacturaTieneCredito(List<FormasPago> formasDePago)
        {
            var formaPagoCredito = formasDePago.Where(r => r.TipoPago == 4 && r.Cobro > 0);

            return formaPagoCredito.HasItems();
        }

        private void SincronizarStockFacturado(List<FacturaDetalle> facturaDetalle, List<Articulo> articulos)
        {
            List<string> articulosProcesados = [];
            foreach (var item in articulos)
            {
                string existeArticulo = articulosProcesados.FirstOrDefault(r => r == item.ArticuloId);
                if (!string.IsNullOrWhiteSpace(existeArticulo)) continue;

                var cantidadArticulosFacturados = facturaDetalle.Where(r => r.ArticuloId == item.ArticuloId).Sum(r => r.Cantidad);

                item.RebajarCantidad(cantidadArticulosFacturados);
                articulosProcesados.Add(item.ArticuloId);
            }
        }

        private Diario MaterializarDiario(FacturaEncabezado facturaEncabezado)
        {
            return new Diario
            {
                BatchId = facturaEncabezado.BatchId,
                CajaId = facturaEncabezado.CajaId,
                Referencia = facturaEncabezado.FacturaId,
                TipoTransaccionId = "Factura"
            };
        }

        private List<FormaPagoDetalle> MaterializarFormaPagoDetalle(List<FormasPago> formasPagos, Batch batch, FacturaEncabezado facturaEncabezado)
        {
            List<FormaPagoDetalle> formaPagoDetalle = [];

            List<FormasPago> formasPagoRecibeCobro = formasPagos.Where(r => r.Cobro > 0).ToList();

            foreach (var item in formasPagoRecibeCobro)
            {
                FormaPagoDetalle nuevoFormaPagoDetalle = new FormaPagoDetalle
                {
                    BatchId = batch.BatchId,
                    FacturaId = facturaEncabezado.FacturaId,
                    FormaPagoId = item.FormaPagoId,
                    Descripcion = item.Descripcion,
                    Monto = item.Cobro,
                    MontoExtranjero = item.MostrarCobro,
                    PagoId = string.Empty
                };
                formaPagoDetalle.Add(nuevoFormaPagoDetalle);
            }

            if (facturaEncabezado.Cambio != 0)
            {
                formaPagoDetalle.Add(new FormaPagoDetalle
                {
                    BatchId = batch.BatchId,
                    FacturaId = facturaEncabezado.FacturaId,
                    FormaPagoId = 098,
                    Descripcion = "Cambio",
                    Monto = facturaEncabezado.Cambio * -1,
                    MontoExtranjero = facturaEncabezado.Cambio * -1,
                    PagoId = string.Empty
                });
            }

            return formaPagoDetalle;
        }
    }
}
