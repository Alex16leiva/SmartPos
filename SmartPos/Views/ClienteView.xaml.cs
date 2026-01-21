using Microsoft.Extensions.DependencyInjection;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for ClienteView.xaml
    /// </summary>
    public partial class ClienteView : System.Windows.Controls.UserControl
    {
        public ClienteView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<SmartPos.ViewModels.ClienteViewModel>();
        }
    }
}
