using Aplicacion.DTOs.Clientes;
using Aplicacion.DTOs.ConfiTienda;
using Aplicacion.DTOs.Factura;
using Aplicacion.DTOs.Finanzas;
using Dominio.Core;
using System.Drawing;

namespace SmartPos.Comunes
{
    public static class Impresiones
    {
        public static BatchDTO Batch { get; set; }
        public static FacturaEncabezadoDTO FacturaEncabezado { get; set; }
        public static List<FacturaDetalleDTO> FacturaDetalle { get; set; }
        public static ConfiguracionTiendaDTO ConfiguracionTienda { get; set; }
        public static ClienteDTO ClienteSeleccionado { get; set; }

        public static System.Drawing.Printing.PrintDocument DocumentoAImprimir = new();
        public static System.Windows.Forms.PrintDialog DialogoImpresion = new();

        #region Métodos de Dibujo (Tickets)

        public static void FacturaListin(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;
            Font fTitulo = new Font("Calibri", 16, FontStyle.Bold);
            Font fNegritaM = new Font("Calibri", 11, FontStyle.Bold);
            Font font7 = new Font("Calibri", 7);
            Font font8 = new Font("Calibri", 8);
            Font font9 = new Font("Calibri", 9);

            StringFormat centerFormat = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            StringFormat rightFormat = new() { Alignment = StringAlignment.Far };

            int offset = 0;
            int movVert = 10;

            // --- ENCABEZADO DE TIENDA ---
            graphic.DrawString(ConfiguracionTienda.Nombre.ToUpper(), fTitulo, Brushes.Black, 135, offset + movVert, centerFormat);
            offset += 25;
            graphic.DrawString(ConfiguracionTienda.Direccion1.GetValueOrEmpty(), font9, Brushes.Black, 150, offset + movVert, centerFormat);
            offset += 15;
            graphic.DrawString($"RTN: {ConfiguracionTienda.RTN.GetValueOrEmpty()}", font9, Brushes.Black, 150, offset + movVert, centerFormat);
            offset += 15;
            graphic.DrawString($"TEL: {ConfiguracionTienda.Telefono1} / {ConfiguracionTienda.Telefono2}", font9, Brushes.Black, 150, offset + movVert, centerFormat);

            // --- INFORMACIÓN DE FACTURA ---
            offset += 30;
            graphic.DrawString($"FACTURA: {FacturaEncabezado.Correlativo}", fNegritaM, Brushes.Black, 150, offset + movVert, centerFormat);
            offset += 20;
            graphic.DrawString($"CAI: {FacturaEncabezado.CAI}", font9, Brushes.Black, 2, offset + movVert);
            offset += 15;
            graphic.DrawString($"Fecha Límite: {FacturaEncabezado.FechaLimiteEmision.ToShortDateString()}", font9, Brushes.Black, 2, offset + movVert);

            // --- SECCIÓN CLIENTE ---
            offset += 20;
            string nombreCliente = "CONSUMIDOR FRINAL";
            string rtnCliente = "S/N";

            if (!string.IsNullOrWhiteSpace(FacturaEncabezado.CampoPersonalizado1))
            {
                nombreCliente = FacturaEncabezado.CampoPersonalizado1;
                rtnCliente = FacturaEncabezado.CampoPersonalizado2.GetValueOrEmpty();
            }
            else if (ClienteSeleccionado != null)
            {
                nombreCliente = $"{ClienteSeleccionado.Nombre} {ClienteSeleccionado.Apellido}".Trim();
                rtnCliente = ClienteSeleccionado.TextoPersonalizado1.GetValueOrEmpty();
            }

            graphic.DrawString($"CLIENTE: {nombreCliente}", font9, Brushes.Black, 2, offset + movVert);
            offset += 15;
            graphic.DrawString($"RTN: {rtnCliente}", font9, Brushes.Black, 2, offset + movVert);
            offset += 15;
            graphic.DrawString($"FECHA: {FacturaEncabezado.FechaTransaccion}", font9, Brushes.Black, 2, offset + movVert);

            // --- DETALLE DE PRODUCTOS ---
            offset += 25;
            graphic.DrawString("CANT", font8, Brushes.Black, 2, offset + movVert);
            graphic.DrawString("DESCRIPCION", font8, Brushes.Black, 65, offset + movVert);
            graphic.DrawString("PRECIO", font8, Brushes.Black, 226, offset + movVert, rightFormat);
            graphic.DrawString("TOTAL", font8, Brushes.Black, 284, offset + movVert, rightFormat);

            offset += 12;
            graphic.DrawLine(Pens.Black, 3, offset + movVert, 285, offset + movVert);

            foreach (var item in FacturaDetalle)
            {
                offset += 5;
                graphic.DrawString(item.Cantidad.ToString(), font8, Brushes.Black, 2, offset + movVert);
                string desc = item.Descripcion.Length > 25 ? item.Descripcion.Substring(0, 22) + "..." : item.Descripcion;
                graphic.DrawString(desc, font8, Brushes.Black, 35, offset + movVert);
                graphic.DrawString(item.Precio.ToString("N2"), font8, Brushes.Black, 226, offset + movVert, rightFormat);
                graphic.DrawString(item.Total.ToString("N2"), font8, Brushes.Black, 284, offset + movVert, rightFormat);
                offset += 15;
                graphic.DrawString($"Cod: {item.ArticuloId}", font7, Brushes.Black, 35, offset + movVert);
                offset += 10;
            }

            // --- TOTALES ---
            offset += 10;
            graphic.DrawLine(Pens.Black, 180, offset + movVert, 285, offset + movVert);
            offset += 5;
            DrawTotalLine(graphic, "SubTotal:", FacturaEncabezado.SubTotal, font9, rightFormat, ref offset, movVert);
            DrawTotalLine(graphic, "Descuento:", FacturaEncabezado.Descuento, font9, rightFormat, ref offset, movVert);
            DrawTotalLine(graphic, "Impuesto:", FacturaEncabezado.Impuesto, font9, rightFormat, ref offset, movVert);

            offset += 5;
            graphic.DrawString("TOTAL:", fNegritaM, Brushes.Black, 213, offset + movVert, rightFormat);
            graphic.DrawString($"L. {FacturaEncabezado.Total:N2}", fNegritaM, Brushes.Black, 284, offset + movVert, rightFormat);

            // --- PIE DE PÁGINA (CONTADO / CRÉDITO) ---
            offset += 40;
            // Aquí validamos si es crédito o contado
            string condicionPago = FacturaEncabezado.TipoFactura == "Credito" ? "FACTURA AL CRÉDITO" : "FACTURA DE CONTADO";
            graphic.DrawString(condicionPago, fNegritaM, Brushes.Black, 150, offset + movVert, centerFormat);

            offset += 15;
            string totalLetras = Utilidades.ConvertirNumeroALetra(FacturaEncabezado.Total.ToString()) + " LEMPIRAS.";
            graphic.DrawString(totalLetras, font7, Brushes.Black, 150, offset + movVert, centerFormat);

            offset += 30;
            graphic.DrawString("GRACIAS POR SU COMPRA", fNegritaM, Brushes.Black, 150, offset + movVert, centerFormat);
        }

        public static void ReporteZ(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fTitulo = new Font("Calibri", 14, FontStyle.Bold);
            Font fNegrita = new Font("Calibri", 10, FontStyle.Bold);
            Font fNormal = new Font("Calibri", 9);
            StringFormat center = new() { Alignment = StringAlignment.Center };
            StringFormat right = new() { Alignment = StringAlignment.Far };

            int offset = 10;
            int colDerecha = 280;

            g.DrawString("REPORTE DE CIERRE Z", fTitulo, Brushes.Black, 140, offset, center);
            offset += 25;
            g.DrawLine(Pens.Black, 5, offset, 285, offset);

            offset += 10;
            g.DrawString($"Lote #: {Batch.BatchId}", fNormal, Brushes.Black, 5, offset);
            offset += 15;
            g.DrawString($"Apertura: {Batch.FechaApertura:dd/MM/yyyy HH:mm}", fNormal, Brushes.Black, 5, offset);
            offset += 15;
            g.DrawString($"Cierre:   {Batch.FechaCierre:dd/MM/yyyy HH:mm}", fNormal, Brushes.Black, 5, offset);

            offset += 25;
            g.DrawString("RESUMEN DE VENTAS", fNegrita, Brushes.Black, 5, offset);
            offset += 20;

            // Totales del Batch
            DrawTotalLine(g, "Ventas Contado:", (Batch.TotalVenta - Batch.VentasCredito), fNormal, right, ref offset, 0);
            DrawTotalLine(g, "Ventas Crédito:", Batch.VentasCredito, fNormal, right, ref offset, 0);
            DrawTotalLine(g, "Devoluciones:", Batch.Devoluciones, fNormal, right, ref offset, 0);
            DrawTotalLine(g, "Impuestos:", Batch.Impuesto, fNormal, right, ref offset, 0);

            offset += 5;
            g.DrawLine(Pens.Black, 150, offset, 285, offset);
            offset += 5;
            g.DrawString("TOTAL NETO:", fNegrita, Brushes.Black, 213, offset, right);
            g.DrawString($"L. {Batch.TotalVenta:N2}", fNegrita, Brushes.Black, colDerecha, offset, right);

            offset += 30;
            g.DrawString("ESTADÍSTICAS", fNegrita, Brushes.Black, 5, offset);
            offset += 15;
            g.DrawString($"Cant. Clientes: {Batch.CantidadClientes}", fNormal, Brushes.Black, 5, offset);

            offset += 50;
            g.DrawString("__________________________", fNormal, Brushes.Black, 140, offset, center);
            offset += 15;
            g.DrawString("FIRMA CAJERO", fNormal, Brushes.Black, 140, offset, center);
        }

        private static void DrawTotalLine(Graphics g, string label, decimal value, Font font, StringFormat format, ref int offset, int movVert)
        {
            g.DrawString(label, font, Brushes.Black, 213, offset + movVert, format);
            g.DrawString(value.ToString("N2"), font, Brushes.Black, 284, offset + movVert, format);
            offset += 15;
        }

        #endregion

        #region Métodos de Ejecución

        public static void ImprimirFacturaContado(string nombreImpresora, bool esVisor, out string errorImpresion)
        {
            DocumentoAImprimir = new System.Drawing.Printing.PrintDocument();
            errorImpresion = string.Empty;
            DocumentoAImprimir.PrintPage += FacturaListin;
            if (!esVisor) errorImpresion = ImprimirDocumento(nombreImpresora);
        }

        public static void ImprimirReporteZ(string nombreImpresora, bool esVisor, out string error)
        {
            DocumentoAImprimir = new System.Drawing.Printing.PrintDocument();
            error = string.Empty;
            DocumentoAImprimir.PrintPage += ReporteZ;
            if (!esVisor) error = ImprimirDocumento(nombreImpresora);
        }

        private static string ImprimirDocumento(string nombreImpresora)
        {
            if (string.IsNullOrWhiteSpace(nombreImpresora))
            {
                if (DialogoImpresion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return TryPrint();
                return "Impresión cancelada";
            }

            DocumentoAImprimir.PrinterSettings.PrinterName = nombreImpresora;
            return TryPrint();
        }

        private static string TryPrint()
        {
            try { DocumentoAImprimir.Print(); return string.Empty; }
            catch { return "Error físico de impresora"; }
        }

        #endregion
    }

    public static class StringExtensions
    {
        public static string GetValueOrEmpty(this string val) => string.IsNullOrWhiteSpace(val) ? string.Empty : val.Trim();
    }
}