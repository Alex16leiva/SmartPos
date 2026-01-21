using Dominio.Core.Extensions;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SmartPos.Views
{
    public partial class ClientesBusquedaView : MetroWindow
    {
        public ClientesBusquedaView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<FacturacionViewModel>();
        }



        private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Forzamos a que el comando se ejecute antes de que el DataGrid use el Enter
                var viewModel = (FacturacionViewModel)this.DataContext;
                if (viewModel.SeleccionarClienteCommand.CanExecute(null))
                {
                    viewModel.SeleccionarClienteCommand.Execute(null);

                    if (viewModel.ClienteSeleccionado.IsNotNull())
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
        }

        private void dgClientes_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
