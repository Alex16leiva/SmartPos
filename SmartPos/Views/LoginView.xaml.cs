using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : MetroWindow
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<LoginViewModel>();
        }
    }
}
