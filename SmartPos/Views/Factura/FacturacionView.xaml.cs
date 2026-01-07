using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartPos.Views.Factura
{
    /// <summary>
    /// Interaction logic for FacturacionView.xaml
    /// </summary>
    public partial class FacturacionView : UserControl
    {
        public FacturacionView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<FacturacionViewModel>();
            Loaded += (s, e) => TxtBusqueda.Focus();
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Pequeño delay para dejar que el Binding actualice el ViewModel primero
                Dispatcher.BeginInvoke(new Action(() => {
                    TxtBusqueda.Focus();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }
    }
}
