using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System.Windows.Controls;

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
        }
    }
}
