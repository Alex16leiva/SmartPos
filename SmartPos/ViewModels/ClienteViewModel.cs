using Aplicacion.DTOs;
using Aplicacion.DTOs.Clientes;
using Aplicacion.Services.ClienteServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes.CommonServices;
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public partial class ClienteViewModel : ObservableObject
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICommonService _commonService;
        private CancellationTokenSource _searchCts;

        public ClienteViewModel(IServiceScopeFactory scopeFactory, ICommonService commonService)
        {
            _scopeFactory = scopeFactory;
            _commonService = commonService;
            _clientes = new ObservableCollection<ClienteDTO>();
            _ = LoadDataAsync();
        }

        [ObservableProperty] private ObservableCollection<ClienteDTO> _clientes;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _textoBusqueda = string.Empty;
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        [ObservableProperty] private bool _isNuevoCliente;
        [ObservableProperty] private bool _isEditFlyoutOpen;
        [ObservableProperty] private ClienteDTO _clienteSeleccionado;

        public List<int> TamañosPagina { get; } = new() { 10, 20, 30, 50, 100 };

        #region Comandos de UI

        [RelayCommand]
        private void NuevoCliente()
        {
            ClienteSeleccionado = new ClienteDTO
            {
                Nombre = string.Empty,
                Apellido = string.Empty,
                NumeroCuenta = string.Empty
            };
            IsNuevoCliente = true;
            IsEditFlyoutOpen = true;
        }

        [RelayCommand]
        private void EditarCliente(ClienteDTO cliente)
        {
            ClienteSeleccionado = cliente;
            IsNuevoCliente = false;
            IsEditFlyoutOpen = true;
        }

        [RelayCommand]
        private void CerrarEdicion() => IsEditFlyoutOpen = false;

        #endregion

        #region Lógica de Guardado (Upsert)

        [RelayCommand]
        private async Task GuardarCambios()
        {
            if (ClienteSeleccionado == null) return;

            // Validación simple
            if (ClienteSeleccionado.Nombre.IsMissingValue())
            {
                _commonService.ShowError("El nombre del cliente es obligatorio.");
                return;
            }

            IsBusy = true;
            var request = new ClienteRequest // Ajustar al nombre real de tu Request
            {
                Cliente = ClienteSeleccionado,
                RequestUserInfo = _commonService.GetRequestInfo()
            };

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _clienteService = scope.ServiceProvider.GetRequiredService<IClienteApplicationService>();

                    ClienteDTO response;
                    if (IsNuevoCliente)
                        response = _clienteService.CrearCliente(request);
                    else
                        response = _clienteService.ActualizarCliente(request);

                    if (response.HasAnyMessage())
                    {
                        _commonService.ShowError(response.Getmessage());
                        return;
                    }

                    _commonService.ShowSuccess(IsNuevoCliente ? "Cliente creado." : "Cambios guardados.");
                    IsEditFlyoutOpen = false;
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                _commonService.ShowError($"Error al guardar: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Busqueda y Paginación

        async partial void OnTextoBusquedaChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(400, _searchCts.Token);
                PaginaActual = 1;
                await LoadDataAsync();
            }
            catch (TaskCanceledException) { }
        }

        partial void OnRegistrosPorPaginaChanged(int value)
        {
            PaginaActual = 1;
            _ = LoadDataAsync();
        }

        [RelayCommand]
        private async Task SiguientePagina()
        {
            if (PaginaActual < TotalPaginas)
            {
                PaginaActual++;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task AnteriorPagina()
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsBusy = true;
            var request = new ClienteRequest
            {
                QueryInfo = ObtenerQueryInfoCliente()
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var _clienteService = scope.ServiceProvider.GetRequiredService<IClienteApplicationService>();
                var result = _clienteService.ObtenerCliente(request);

                Clientes = new ObservableCollection<ClienteDTO>(result.Items);
                TotalPaginas = result.PageCount;
            }
            IsBusy = false;
        }

        private QueryInfo ObtenerQueryInfoCliente()
        {
            var queryInfo = new QueryInfo
            {
                PageIndex = PaginaActual - 1,
                PageSize = RegistrosPorPagina,
                SortFields = new List<string> { "Nombre" },
                Ascending = true,
                Predicate = string.Empty,
                ParamValues = []
            };

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                // Predicado dinámico para Clientes
                queryInfo.Predicate = "Nombre.Contains(@0) OR Apellido.Contains(@0) OR NumeroCuenta.Contains(@0) OR TextoPersonalizado1.Contains(@0)";
                queryInfo.ParamValues = new object[] { TextoBusqueda };
            }

            return queryInfo;
        }

        #endregion
    }
}