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
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<FacturaDetalleDTO> _facturaDetalle = new();
        [ObservableProperty] private FacturaEncabezadoDTO _encabezado = new();
        [ObservableProperty] private VendedorDTO _vendedorSeleccionado = new();
        [ObservableProperty] private string _busquedaArticulo = string.Empty;
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
                ActualizarTotales();
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
