using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<MainViewModel>();
        }
    }
}