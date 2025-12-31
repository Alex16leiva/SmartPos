using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.Services.ArticuloServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Context.Entidades.Articulos;
using SmartPos.DTOs.Articulos;
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly IArticuloApplicationService _articuloApplicationService;
        public InventarioViewModel(IArticuloApplicationService articuloApplicationService)
        {
            _articuloApplicationService = articuloApplicationService;
            _articulos = new ObservableCollection<ArticulosDTO>();
            LoadDataAsync();
        }

        [ObservableProperty] private ObservableCollection<ArticulosDTO> _articulos;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _textoBusqueda = string.Empty;
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        public List<int> TamañosPagina { get; } = new() { 10, 20, 30, 50, 100 };
        private CancellationTokenSource _searchCts;

        [ObservableProperty] private bool _isEditFlyoutOpen;
        [ObservableProperty] private ArticulosDTO _articuloSeleccionado;

        [RelayCommand]
        private void EditarArticulo(ArticulosDTO articulo)
        {
            // Hacemos una copia o pasamos la referencia
            ArticuloSeleccionado = articulo;
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


            // 2. Ejecutamos la actualización en un hilo de fondo para no congelar la UI
            //await Task.Run(async () =>
            //{
            //    // Supongamos que tu repositorio tiene un método Update genérico
            //    var articulo = await _genericRepository.GetSingleAsync<Articulo>(r => r.ArticuloId == ArticuloSeleccionado.ArticuloId);

            //    if (articulo.IsNull())
            //    {
            //        return;
            //    }

            //    // Actualizamos las propiedades necesarias
            //    articulo.Descripcion = ArticuloSeleccionado.Descripcion;
            //    articulo.DescripcionExtendida = ArticuloSeleccionado.DescripcionExtendida;
            //    articulo.Cantidad = ArticuloSeleccionado.Cantidad;
            //    articulo.Costo = ArticuloSeleccionado.Costo;
            //    articulo.UltimoCosto = ArticuloSeleccionado.UltimoCosto;
            //    articulo.Precio = ArticuloSeleccionado.Precio;
            //    articulo.PrecioA = ArticuloSeleccionado.PrecioA;

            //});

            // 3. Cerramos el panel lateral
            IsEditFlyoutOpen = false;

            // 4. OPCIONAL: Mostrar una notificación o refrescar la lista
            // Como el objeto está bindeado, el DataGrid se actualizará solo, 
            // pero si quieres asegurar la persistencia visual desde DB:
            // await LoadDataAsync(); 
            // 5. Liberamos la UI
            IsBusy = false;

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

