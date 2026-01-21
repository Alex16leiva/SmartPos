using Aplicacion.DTOs.Clientes;
using Aplicacion.DTOs.ConfiTienda;
using Aplicacion.DTOs.Factura;
using Aplicacion.DTOs.Finanzas;
using Dominio.Core;

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

        public static void FacturaListin(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;
            // Definición de fuentes
            Font fTitulo = new Font("Calibri", 16, FontStyle.Bold);
            Font fNegritaM = new Font("Calibri", 11, FontStyle.Bold);
            Font font7 = new Font("Calibri", 7);
            Font font8 = new Font("Calibri", 8);
            Font font9 = new Font("Calibri", 9);

            StringFormat centerFormat = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            StringFormat rightFormat = new() { Alignment = StringAlignment.Far };

            int offset = 0;
            int movVert = 10; // Ajustado para iniciar desde arriba

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

            // --- SECCIÓN CLIENTE (MEJORADA) ---
            offset += 20;
            string nombreCliente = "CONSUMIDOR FINAL";
            string rtnCliente = "S/N";

            if (!string.IsNullOrWhiteSpace(FacturaEncabezado.CampoPersonalizado1)) // Prioridad a datos manuales
            {
                nombreCliente = FacturaEncabezado.CampoPersonalizado1;
                rtnCliente = FacturaEncabezado.CampoPersonalizado2.GetValueOrEmpty();
            }
            else if (ClienteSeleccionado != null)
            {
                nombreCliente = $"{ClienteSeleccionado.Nombre} {ClienteSeleccionado.Apellido}".Trim();
                rtnCliente = ClienteSeleccionado.TextoPersonalizado1.GetValueOrEmpty();

                if (ClienteSeleccionado.NumeroCuenta != "000")
                {
                    graphic.DrawString($"Cod. Cliente: {ClienteSeleccionado.NumeroCuenta}", font9, Brushes.Black, 2, offset + movVert);
                    offset += 15;
                }
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

                // Truncar descripción si es muy larga
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

            // --- PIE DE PÁGINA ---
            offset += 40;
            string totalLetras = Utilidades.ConvertirNumeroALetra(FacturaEncabezado.Total.ToString()) + " LEMPIRAS.";
            graphic.DrawString(totalLetras, font7, Brushes.Black, 150, offset + movVert, centerFormat);

            offset += 30;
            graphic.DrawString("GRACIAS POR SU COMPRA", fNegritaM, Brushes.Black, 150, offset + movVert, centerFormat);
        }

        private static void DrawTotalLine(Graphics g, string label, decimal value, Font font, StringFormat format, ref int offset, int movVert)
        {
            g.DrawString(label, font, Brushes.Black, 213, offset + movVert, format);
            g.DrawString(value.ToString("N2"), font, Brushes.Black, 284, offset + movVert, format);
            offset += 15;
        }

        // --- MÉTODOS DE IMPRESIÓN ---
        internal static void ImprimirFacturaContado(string nombreImpresora, bool esVisor, out string errorImpresion)
        {
            DocumentoAImprimir = new System.Drawing.Printing.PrintDocument();
            errorImpresion = string.Empty;
            DocumentoAImprimir.PrintPage += FacturaListin;
            if (!esVisor) errorImpresion = ImprimirDocumento(nombreImpresora);
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
    }

    // --- EXTENSIONES NECESARIAS ---
    public static class StringExtensions
    {
        public static string GetValueOrEmpty(this string val) => string.IsNullOrWhiteSpace(val) ? string.Empty : val.Trim();
    }
}