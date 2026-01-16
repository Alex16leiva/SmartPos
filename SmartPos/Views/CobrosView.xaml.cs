using MahApps.Metro.Controls;
using SmartPos.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for CobrosView.xaml
    /// </summary>
    public partial class CobrosView : MetroWindow
    {
        public CobrosView()
        {
            InitializeComponent();
        }

        private void dgCobro_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Pequeño delay para que el binding termine de actualizar el modelo antes de sumar
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                if (this.DataContext is FacturacionViewModel vm)
                {
                    vm.CalcularTotalRecibido();
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Validamos que el pago sea suficiente (opcional)
            this.DialogResult = true;
            this.Close();
        }

        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            var datagrid = sender as System.Windows.Controls.DataGrid;
            if (dgCobro.Columns == null)
            {
                return;
            }
            foreach (var item in dgCobro.Columns)
            {
                item.CellStyle = SetStyleAzul();
            }

            datagrid.CurrentCell.Column.CellStyle = SetStyleRojo();
        }

        private Style SetStyleAzul()
        {
            var stringReader = new StringReader("<Style xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" TargetType=\"{x:Type DataGridCell}\">  <Setter Property=\"Foreground\" Value=\"Black\"></Setter> </Style>");
            var xmlReader = XmlReader.Create(stringReader);
            var style = (Style)System.Windows.Markup.XamlReader.Load(xmlReader);
            return style;
        }

        private Style SetStyleRojo()
        {
            var stringReader = new StringReader("<Style xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" TargetType=\"{x:Type DataGridCell}\"> <Setter Property=\"Background\" Value=\"White\"></Setter> <Setter Property=\"Foreground\" Value=\"Black\"></Setter> </Style>");
            var xmlReader = XmlReader.Create(stringReader);
            var style = (Style)System.Windows.Markup.XamlReader.Load(xmlReader);
            return style;
        }
    }
}
