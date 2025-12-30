using Aplicacion.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartPos.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly SecurityAplicationService _securityAplicationService;

        public LoginViewModel(SecurityAplicationService securityAplicationService)
        {
            _securityAplicationService = securityAplicationService;
        }


    }
}
