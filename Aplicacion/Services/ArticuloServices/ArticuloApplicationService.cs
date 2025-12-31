using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Seguridad;
using Aplicacion.Helpers;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
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
    }
}
