using Aplicacion.DTOs.Articulos;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for InventarioView.xaml
    /// </summary>
    public partial class InventarioView : UserControl
    {
        public InventarioView()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetService<InventarioViewModel>();
        }

        private void BtnKardex_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtenemos el artículo seleccionado del DataGrid
            // Asumiendo que el objeto de la fila es 'ArticuloDTO'
            var boton = sender as Button;
            var articuloSeleccionado = boton.DataContext as ArticulosDTO;

            if (articuloSeleccionado != null)
            {
                // 2. Instanciamos la nueva pantalla pasándole los datos
                // Usamos 'this' para que la ventana principal sea la dueña (Owner)
                var ventanaKardex = new KardexView(articuloSeleccionado.ArticuloId);
                ventanaKardex.Owner = Window.GetWindow(this);

                // 3. La mostramos como diálogo (bloquea la anterior hasta cerrar)
                ventanaKardex.ShowDialog();
            }
        }
    }
}
