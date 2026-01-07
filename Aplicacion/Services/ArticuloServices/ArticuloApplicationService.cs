using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Seguridad;
using Aplicacion.Helpers;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;
using SmartPos.DTOs.Articulos;

namespace Aplicacion.Services.ArticuloServices
{
    public class ArticuloApplicationService : IArticuloApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        public ArticuloApplicationService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public SearchResult<ArticulosDTO> ObtenerArticulos(ArticuloRequest request)
        {
            DynamicFilter dynamicFilter = DynamicFilterFactory.CreateDynamicFilter(request.QueryInfo);

            // Llamada a tu repositorio genérico
            PagedCollection articulos = _genericRepository.GetPagedAndFiltered<Articulo>(dynamicFilter);

            return new SearchResult<ArticulosDTO>
            {
                PageCount = articulos.PageCount,
                ItemCount = articulos.ItemCount,
                TotalItems = articulos.TotalItems,
                PageIndex = articulos.PageIndex,
                Items = (from qry in articulos.Items as IEnumerable<Articulo> select MapArticulo(qry)).ToList(),
            };
        }

        private static ArticulosDTO MapArticulo(Articulo qry)
        {
            return new ArticulosDTO
            {
                ArticuloId = qry.ArticuloId,
                Cantidad = qry.Cantidad,
                CantidadComprometida = qry.CantidadComprometida,
                CategoriaId = qry.CategoriaId,
                Costo = qry.Costo,
                DepartamentoId = qry.DepartamentoId,
                Descripcion = qry.Descripcion,
                DescripcionExtendida = qry.DescripcionExtendida,
                FechaFinalOferta = qry.FechaFinalOferta,
                FechaInicioOferta = qry.FechaInicioOferta,
                FechaTransaccion = qry.FechaTransaccion,
                ImagenRuta = qry.ImagenRuta,
                ImpuestoId = qry.ImpuestoId,
                Inactivo = qry.Inactivo,
                Notas = qry.Notas,
                OfertaActiva = qry.OfertaActiva,
                Precio = qry.Precio,
                PrecioA = qry.PrecioA,
                PrecioB = qry.PrecioB,
                PrecioC = qry.PrecioC,
                PrecioD = qry.PrecioD,
                PrecioE = qry.PrecioE,
                PrecioOferta = qry.PrecioOferta,
                ProveedorId = qry.ProveedorId,
                PuntoReorden = qry.PuntoReorden,
                TextoPersonalizado1 = qry.TextoPersonalizado1,
                TextoPersonalizado2 = qry.TextoPersonalizado2,
                TextoPersonalizado3 = qry.TextoPersonalizado3,
                TipoArticuloId = qry.TipoArticuloId,
                UltimaVenta = qry.UltimaVenta,
                UltimoCosto = qry.UltimoCosto,
                UltimoRecibo = qry.UltimoRecibo,
                UnidadMedida = qry.UnidadMedida,
                
            };
        }

        public async Task<ArticulosDTO> CrearArticuloAsync(ArticuloRequest request)
        {
            var existe = await _genericRepository.GetSingleAsync<Articulo>(a => a.ArticuloId == request.Articulo.ArticuloId);
            if (existe != null)
            {
                return new ArticulosDTO { Message = $"El código de artículo {request.Articulo.ArticuloId} ya está en uso." };
            }

            var nuevoArticulo = new Articulo
            {
                ArticuloId = request.Articulo.ArticuloId,
                Descripcion = request.Articulo.Descripcion,
                DescripcionExtendida = request.Articulo.DescripcionExtendida,
                Precio = request.Articulo.Precio,
                Costo = request.Articulo.Costo,
                Cantidad = request.Articulo.Cantidad,
                UltimoCosto = 0, // Por ser nuevo
            };

            _genericRepository.Add(nuevoArticulo);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("Creacion de articulo");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return request.Articulo;
        }

        public async Task<ArticulosDTO> ActualizarArticuloAsync(ArticuloRequest request)
        {
            // 1. Buscar el artículo original en la base de datos
            var articuloExistente = await _genericRepository.GetSingleAsync<Articulo>(a =>
                a.ArticuloId == request.Articulo.ArticuloId);

            if (articuloExistente == null)
            {
                return new ArticulosDTO { Message = $"No se encontró el artículo con código {request.Articulo.ArticuloId}." };
            }

            // 3. Mapeo de cambios desde el DTO a la Entidad
            articuloExistente.Descripcion = request.Articulo.Descripcion;
            articuloExistente.DescripcionExtendida = request.Articulo.DescripcionExtendida;
            articuloExistente.Precio = request.Articulo.Precio;
            articuloExistente.ActualizarCosto(request.Articulo.Costo);
            // Nota: La cantidad usualmente se maneja por movimientos, 
            // pero para esta edición directa la actualizamos:
            articuloExistente.Cantidad = request.Articulo.Cantidad;

            
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("ActualizacionArticulo");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return request.Articulo;
        }

        public async Task<ArticulosDTO> RegistrarMovimientoAsync(ArticuloRequest request)
        {
            var articulo = await _genericRepository.GetSingleAsync<Articulo>(a =>
                a.ArticuloId == request.Articulo.ArticuloId);

            if (articulo.IsNull()) return new ArticulosDTO { Message = $"Articulo {request.Articulo.ArticuloId} no encontrado" };

            // 1. Guardamos estado anterior para el histórico
            decimal cantidadAnterior = articulo.Cantidad;
            decimal cantidadMovimiento = Math.Abs(request.InventarioMovimiento.CantidadMovimiento);
            if (request.InventarioMovimiento.TipoMovimiento == "SALIDA")
            {
                if (articulo.RebajarCantidad(cantidadMovimiento))
                {
                    return new ArticulosDTO 
                    {
                        Message = $"Error: No hay suficiente stock. Disponible: {articulo.Cantidad}, Intento: {cantidadMovimiento}"
                    };
                }
            }

            if (request.InventarioMovimiento.TipoMovimiento == "ENTRADA")
            {
                articulo.AjusteDeAumento(cantidadMovimiento);
                articulo.ActualizarCosto(request.InventarioMovimiento.CostoUnitario);
            }

            // 3. Creamos el registro del movimiento
            var movimiento = new InventarioMovimiento
            {
                ArticuloId = articulo.ArticuloId,
                CantidadAnterior = cantidadAnterior,
                CantidadMovimiento = cantidadMovimiento,
                CantidadNueva = articulo.Cantidad,
                TipoMovimiento = request.InventarioMovimiento.TipoMovimiento,
                Referencia = request.InventarioMovimiento.Referencia,
                Notas = request.InventarioMovimiento.Notas,
                CostoUnitario = request.InventarioMovimiento.CostoUnitario,
            };

            _genericRepository.Add(movimiento);

            // 4. Commit con tu infraestructura de auditoría
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("AddInventoryMovement");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return request.Articulo;
        }
    
        public async Task<ArticuloResponse> ObtenerKardexArticuloAsync (string articuloId)
        {
            List<string> includes = ["InventarioMovimiento"];
            var articulo = await _genericRepository.GetSingleAsync<Articulo>(a =>
                a.ArticuloId == articuloId, includes);

            if (articulo.IsNull()) return new ArticuloResponse { Message = $"Articulo { articuloId } no encontrado" };

            var inventarioMovimientosDto = MapearInventarioMovimientosDto(articulo.InventarioMovimiento.ToList());

            return new ArticuloResponse
            {
                InventarioMovimientos = inventarioMovimientosDto,
                Articulo = MapArticulo(articulo)
            };
        }

        private List<InventarioMovimientoDTO> MapearInventarioMovimientosDto(List<InventarioMovimiento> inventarioMovimientos)
        {
            return [.. inventarioMovimientos.Select(item => MapearInventarioMovimientoDto(item))];
        }

        private static InventarioMovimientoDTO MapearInventarioMovimientoDto(InventarioMovimiento item)
        {
            return new InventarioMovimientoDTO
            {
                ArticuloId = item.ArticuloId,
                CantidadAnterior = item.CantidadAnterior,
                CantidadMovimiento = item.CantidadMovimiento,
                CantidadNueva = item.CantidadNueva,
                CostoUnitario = item.CostoUnitario,
                FechaTransaccion = item.FechaTransaccion,
                InventarioMovimientoId = item.InventarioMovimientoId,
                Notas = item.Notas,
                Referencia = item.Referencia,
                TipoMovimiento = item.TipoMovimiento,
                ModificadoPor = item.ModificadoPor,
                EsEntrada = item.TipoMovimiento == "ENTRADA"
            };
        }
    }
}
