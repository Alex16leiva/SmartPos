using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System.Windows.Controls;

namespace SmartPos.Views.Seguridad
{
    /// <summary>
    /// Interaction logic for SeguridadView.xaml
    /// </summary>
    public partial class SeguridadView : UserControl
    {
        public SeguridadView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<SeguridadViewModel>();
        }
    }
}
