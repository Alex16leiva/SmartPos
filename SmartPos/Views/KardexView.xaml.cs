using Aplicacion.DTOs.Articulos;
using Aplicacion.Services.ArticuloServices;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SmartPos.Comunes.CommonServices;
using System.IO;
using System.Windows;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for KardexView.xaml
    /// </summary>
    public partial class KardexView : MetroWindow
    {
        private readonly string _articuloId;
        public KardexView(string articuloId)
        {
            InitializeComponent();
            _articuloId = articuloId;
            TxtNombreArticulo.Text = articuloId;

            CargarDatos();
        }

        private async void CargarDatos()
        {            
            var service = App.ServiceProvider.GetRequiredService<IArticuloApplicationService>();
            var data = await service.ObtenerKardexArticuloAsync(_articuloId);
            DgKardex.ItemsSource = data.InventarioMovimientos;

            // El último saldo en la lista (que está ordenada por fecha DESC) es el stock actual
            TxtStockActual.Text = data.Articulo.Cantidad.ToString("N2") ?? "0.00";            
        }

        private async void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            var lista = DgKardex.ItemsSource as IEnumerable<InventarioMovimientoDTO>;
            if (lista != null && lista.Any())
            {
                await ExportarKardexAExcel(lista);
            }
        }


        private async Task ExportarKardexAExcel(IEnumerable<InventarioMovimientoDTO> datos)
        {
            // 1. Configurar el diálogo de guardado
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"Kardex_{_articuloId}_{DateTime.Now:yyyyMMdd}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Intenta esto si lo anterior falla
                ExcelPackage.License.SetNonCommercialPersonal("SmartPos Project");
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Historial");

                    // 2. Encabezados con estilo
                    string[] headers = { "Fecha", "Operación", "Referencia", "Cantidad Anterior", "Cantidad Movimiento", "Cantidad Nueva", "Usuario", "Costo" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = sheet.Cells[1, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // 3. Llenado de datos
                    int row = 2;
                    foreach (var item in datos)
                    {
                        sheet.Cells[row, 1].Value = item.FechaTransaccion?.ToString("dd/MM/yyyy HH:mm");
                        sheet.Cells[row, 2].Value = item.TipoMovimiento;
                        sheet.Cells[row, 3].Value = item.Referencia;
                        sheet.Cells[row, 4].Value = item.CantidadAnterior;
                        sheet.Cells[row, 5].Value = item.CantidadMovimiento;
                        sheet.Cells[row, 6].Value = item.CantidadNueva;
                        sheet.Cells[row, 7].Value = item.ModificadoPor;
                        sheet.Cells[row, 8].Value = item.CostoUnitario;

                        // Color condicional en la columna Cantidad (Columna 4)
                        if (item.EsEntrada)
                            sheet.Cells[row, 4].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        else
                            sheet.Cells[row, 4].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        row++;
                    }

                    sheet.Cells.AutoFitColumns();

                    // 4. Guardar archivo
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, package.GetAsByteArray());

                    // Usamos tu servicio de mensajes para confirmar
                    var commonService = App.ServiceProvider.GetRequiredService<ICommonService>();
                    commonService.ShowSuccess("El archivo se ha exportado correctamente.");
                }
            }
        }
    }
}
