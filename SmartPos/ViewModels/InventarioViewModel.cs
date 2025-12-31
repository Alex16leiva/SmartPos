using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.Services.ArticuloServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Context.Entidades.Articulos;
using Dominio.Core.Extensions;
using SmartPos.Comunes.CommonServices;
using SmartPos.DTOs.Articulos;
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly IArticuloApplicationService _articuloApplicationService;
        private readonly ICommonService _commonService;
        public InventarioViewModel(IArticuloApplicationService articuloApplicationService, ICommonService commonService)
        {
            _articuloApplicationService = articuloApplicationService;
            _commonService = commonService;
            _articulos = new ObservableCollection<ArticulosDTO>();
            LoadDataAsync();
        }

        [ObservableProperty] private ObservableCollection<ArticulosDTO> _articulos;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _textoBusqueda = string.Empty;
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        [ObservableProperty] private bool _isNuevoArticulo;
        public List<int> TamañosPagina { get; } = new() { 10, 20, 30, 50, 100 };
        private CancellationTokenSource _searchCts;

        [ObservableProperty] private bool _isEditFlyoutOpen;
        [ObservableProperty] private ArticulosDTO _articuloSeleccionado;

        [RelayCommand]
        private void NuevoArticulo()
        {
            // Preparamos un DTO vacío para el formulario
            ArticuloSeleccionado = new ArticulosDTO
            {
                ArticuloId = string.Empty,
                Descripcion = string.Empty,
                Cantidad = 0,
                Precio = 0,
                Costo = 0
            };
            IsNuevoArticulo = true;
            IsEditFlyoutOpen = true;
        }

        [RelayCommand]
        private void EditarArticulo(ArticulosDTO articulo)
        {
            // Hacemos una copia o pasamos la referencia
            ArticuloSeleccionado = articulo;
            IsNuevoArticulo = false;
            IsEditFlyoutOpen = true;
        }

        [RelayCommand]
        private void CerrarEdicion() => IsEditFlyoutOpen = false;



        async partial void OnTextoBusquedaChanged(string value)
        {
            // Cancelar la búsqueda pendiente si el usuario sigue escribiendo
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                // Esperar 400ms de silencio antes de disparar la consulta
                await Task.Delay(400, _searchCts.Token);

                PaginaActual = 1;
                await LoadDataAsync();
            }
            catch (TaskCanceledException) { /* Ignorar cancelación */ }
        }

        [RelayCommand]
        private async Task GuardarCambios()
        {
            if (ArticuloSeleccionado == null) return;

            // 1. Bloqueamos la UI (Activa el ProgressRing que ya tienes en el XAML)
            IsBusy = true;
            var request = new ArticuloRequest
            {
                Articulo = ArticuloSeleccionado,
                RequestUserInfo = _commonService.GetRequestInfo() // Obtenemos el usuario logueado
            };
            if (IsNuevoArticulo)
            {
                var resultado = await _articuloApplicationService.CrearArticuloAsync(request);

                // 3. Validar si el servicio retornó un mensaje de error
                if (resultado.Message.HasValue())
                {
                    _commonService.ShowError(resultado.Message);
                    return;
                }
                _commonService.ShowSuccess("Artículo creado correctamente");
            }
            else
            {
                var resultado = await _articuloApplicationService.ActualizarArticuloAsync(request);

                // 3. Validar si el servicio retornó un mensaje de error
                if (resultado.Message.HasValue())
                {
                    _commonService.ShowError(resultado.Message);
                    return;
                }
                _commonService.ShowSuccess("Cambios guardados");
            }

                // 3. Cerramos el panel lateral
                IsEditFlyoutOpen = false;

            
            IsBusy = false;
            await LoadDataAsync();
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

        partial void OnRegistrosPorPaginaChanged(int value)
        {
            // Reiniciamos a la página 1
            PaginaActual = 1;

            // Ejecutamos la carga de datos
            // Usamos el método directamente ya que estamos en el mismo VM
            _ = LoadDataAsync();
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsBusy = true; // Para mostrar el ProgressRing de MahApps

            
                var request = new ArticuloRequest
                {
                    QueryInfo = ObtenerQueryInfoUsuarios()
                };

            var result = _articuloApplicationService.ObtenerArticulos(request);
                // Actualizamos la UI en el hilo principal

                    Articulos = new ObservableCollection<ArticulosDTO>(result.Items);

                    // Actualizamos el total de páginas basándonos en la respuesta del backend
                    // Si PagedCollection tiene TotalCount, úsalo aquí:
                    TotalPaginas = result.PageCount;
            
            IsBusy = false; // Ocultar el ProgressRing
        }

        private QueryInfo ObtenerQueryInfoUsuarios()
        {
            var queryInfo = new QueryInfo
            {
                PageIndex = PaginaActual - 1,
                PageSize = RegistrosPorPagina, // Coincide con tu lógica
                SortFields = new List<string> { "FechaTransaccion" }, // Cambié FechaTransaccion por Id por si Articulo no la tiene
                Ascending = false,
                Predicate = string.Empty,
                ParamValues = []
            };

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                // Ajustado para buscar por Nombre o Id del Artículo
                queryInfo.Predicate = "ArticuloId.ToString().Contains(@0) OR Descripcion.Contains(@0)";
                queryInfo.ParamValues = new object[] { TextoBusqueda };
            }

            return queryInfo;
        }
    }
}

