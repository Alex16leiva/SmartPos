using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;

namespace SmartPos.Views.Seguridad
{
    /// <summary>
    /// Interaction logic for SeguridadView.xaml
    /// </summary>
    public partial class SeguridadView : System.Windows.Controls.UserControl
    {
        public SeguridadView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<SeguridadViewModel>();
        }
    }
}
