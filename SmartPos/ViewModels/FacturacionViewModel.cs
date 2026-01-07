using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Comunes;
using Aplicacion.DTOs.Factura;
using Aplicacion.Services.ArticuloServices;
using Aplicacion.Services.Factura;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes.CommonServices;
using SmartPos.DTOs.Articulos;
using SmartPos.Views;
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<FacturaDetalleDTO> _facturaDetalle = new();
        [ObservableProperty] private FacturaEncabezadoDTO _encabezado = new();
        [ObservableProperty] private VendedorDTO _vendedorSeleccionado = new();
        [ObservableProperty] private string _busquedaArticulo = string.Empty;

        [ObservableProperty] private ObservableCollection<ArticulosDTO> _articulosBusqueda = new();
        [ObservableProperty] private ArticulosDTO? _articuloBusquedaSeleccionado;
        [ObservableProperty] private string _textoBusquedaModal = string.Empty;
        [ObservableProperty] private bool _isBusy;

        // Propiedades de Paginación
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        private CancellationTokenSource? _searchCts;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICommonService _commonService;
        public FacturacionViewModel(IServiceScopeFactory scopeFactory, ICommonService commonService)
        {
            _commonService = commonService;
            _scopeFactory = scopeFactory;
        }


        [RelayCommand]
        private void AgregarArticulo(string codigo)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var articuloAppService = scope.ServiceProvider.GetRequiredService<IFacturaApplicationService>();
                AgregarArticuloRequest request = new AgregarArticuloRequest
                {
                    ArticuloId = BusquedaArticulo.Trim(),
                    Vendedor = VendedorSeleccionado,
                    FacturasDetalle = FacturaDetalle.ToList()
                };
                var response = articuloAppService.AgregarArticuloAFactura(request);

                if (response.Message.HasValue())
                {
                    _commonService.ShowWarning(response.Message);
                    return;
                }
                FacturaDetalle = new ObservableCollection<FacturaDetalleDTO>(response.FacturaDetalle);
                BusquedaArticulo = string.Empty;
                ActualizarTotales();
            }
        }

        [RelayCommand]
        public async Task LoadDataBusquedaAsync()
        {
            IsBusy = true;

            var request = new ArticuloRequest
            {
                QueryInfo = new QueryInfo
                {
                    PageIndex = PaginaActual - 1,
                    PageSize = RegistrosPorPagina,
                    SortFields = new List<string> { "Descripcion" },
                    Ascending = true,
                    Predicate = !string.IsNullOrWhiteSpace(TextoBusquedaModal)
                                ? "ArticuloId.ToString().Contains(@0) OR Descripcion.Contains(@0)"
                                : string.Empty,
                    ParamValues = !string.IsNullOrWhiteSpace(TextoBusquedaModal)
                                  ? new object[] { TextoBusquedaModal }
                                  : Array.Empty<object>()
                }
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var articuloService = scope.ServiceProvider.GetRequiredService<IArticuloApplicationService>();
                var result = articuloService.ObtenerArticulos(request);

                ArticulosBusqueda = new ObservableCollection<ArticulosDTO>(result.Items);
                TotalPaginas = result.PageCount;
            }
            IsBusy = false;
        }

        // Debouncing para la búsqueda en el modal
        async partial void OnTextoBusquedaModalChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(400, _searchCts.Token);
                PaginaActual = 1;
                await LoadDataBusquedaAsync();
            }
            catch (TaskCanceledException) { }
        }


        [RelayCommand]
        private void AbrirBuscadorArticulos()
        {
            // Reiniciar búsqueda al abrir
            TextoBusquedaModal = string.Empty;
            PaginaActual = 1;
            _ = LoadDataBusquedaAsync();

            var vistaBusqueda = new BusquedaArticuloView();
            vistaBusqueda.DataContext = this;

            if (vistaBusqueda.ShowDialog() == true)
            {
                if (ArticuloBusquedaSeleccionado != null)
                {
                    BusquedaArticulo = ArticuloBusquedaSeleccionado.ArticuloId;
                    AgregarArticulo(BusquedaArticulo);
                }
            }
        }

        private void ActualizarTotales()
        {
            Encabezado.Total = FacturaDetalle.Sum(x => x.Total);
            Encabezado.Impuesto = FacturaDetalle.Sum(x => x.Impuesto);
            
            OnPropertyChanged(nameof(Encabezado)); // Notifica a la UI
        }
    }
}
