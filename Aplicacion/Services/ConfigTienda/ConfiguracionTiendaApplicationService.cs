using Aplicacion.DTOs.ConfiTienda;
using Aplicacion.Helpers;
using Dominio.Context.Entidades;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;

namespace Aplicacion.Services.ConfiTienda
{
    public class ConfiguracionTiendaApplicationService : IConfiguracionTiendaApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        public ConfiguracionTiendaApplicationService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public ConfiguracionTiendaDTO ObtenerConfiguracionTienda(ConfiguracionTiendaRequest request)
        {
            var configuracionTiendas = _genericRepository.GetAll<ConfiguracionTienda>();
            var configuracionTienda = configuracionTiendas.FirstOrDefault();
            if (configuracionTienda.IsNull())
            {

                if (!request.EsAdminPos)
                {
                    return new ConfiguracionTiendaDTO
                    {
                        Message = "La configuración de la tienda no está establecida. Por favor, contacte al administrador del sistema."
                    };
                }
                else
                {
                    return new ConfiguracionTiendaDTO();
                }
            }

            return new ConfiguracionTiendaDTO
            {
                Id = configuracionTienda.Id,
                Ciudad = configuracionTienda.Ciudad,
                CodigoZip = configuracionTienda.CodigoZip,
                CorreoElectronico = configuracionTienda.CorreoElectronico,
                Direccion1 = configuracionTienda.Direccion1,
                Direccion2 = configuracionTienda.Direccion2,
                Estado = configuracionTienda.Estado,
                Nombre = configuracionTienda.Nombre,
                Pais = configuracionTienda.Pais,
                Personalizado1 = configuracionTienda.Personalizado1,
                Personalizado2 = configuracionTienda.Personalizado2,
                Personalizado3 = configuracionTienda.Personalizado3,
                Personalizado4 = configuracionTienda.Personalizado4,
                RTN = configuracionTienda.RTN,
                Telefono1 = configuracionTienda.Telefono1,
                Telefono2 = configuracionTienda.Telefono2,
                Telefono3 = configuracionTienda.Telefono3
            };
        }

        public string GuardarCambiosConfiguracionTienda(ConfiguracionTiendaRequest request)
        {
            var configuracionTienda = _genericRepository.GetSingle<ConfiguracionTienda>(r => r.Id == request.ConfiguracionTienda.Id);

            if (configuracionTienda.IsNull())
            {
                ConfiguracionTienda configTienda = new ConfiguracionTienda
                {
                    Nombre = request.ConfiguracionTienda.Nombre,
                    Ciudad = request.ConfiguracionTienda.Ciudad,
                    Estado = request.ConfiguracionTienda.Estado,
                    Direccion1 = request.ConfiguracionTienda.Direccion1,
                    Direccion2 = request.ConfiguracionTienda.Direccion2,
                    CodigoZip = request.ConfiguracionTienda.CodigoZip,
                    CorreoElectronico = request.ConfiguracionTienda.CorreoElectronico,
                    Pais = request.ConfiguracionTienda.Pais,
                    Personalizado1 = request.ConfiguracionTienda.Personalizado1,
                    Personalizado2 = request.ConfiguracionTienda.Personalizado2,
                    Personalizado3 = request.ConfiguracionTienda.Personalizado3,
                    Personalizado4 = request.ConfiguracionTienda.Personalizado4,
                    RTN = request.ConfiguracionTienda.RTN,
                    Telefono1 = request.ConfiguracionTienda.Telefono1,
                    Telefono2 = request.ConfiguracionTienda.Telefono2,
                    Telefono3 = request.ConfiguracionTienda.Telefono3
                };

                _genericRepository.Add(configTienda);
                TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("CrearCliente");
                _genericRepository.UnitOfWork.Commit(transactionInfo);

                return "La tienda se creo";
            }

            configuracionTienda.Nombre = request.ConfiguracionTienda.Nombre;
            configuracionTienda.Ciudad = request.ConfiguracionTienda.Ciudad;
            configuracionTienda.Estado = request.ConfiguracionTienda.Estado;
            configuracionTienda.Direccion1 = request.ConfiguracionTienda.Direccion1;
            configuracionTienda.Direccion2 = request.ConfiguracionTienda.Direccion2;
            configuracionTienda.CodigoZip = request.ConfiguracionTienda.CodigoZip;
            configuracionTienda.CorreoElectronico = request.ConfiguracionTienda.CorreoElectronico;
            configuracionTienda.Pais = request.ConfiguracionTienda.Pais;
            configuracionTienda.Personalizado1 = request.ConfiguracionTienda.Personalizado1;
            configuracionTienda.Personalizado2 = request.ConfiguracionTienda.Personalizado2;
            configuracionTienda.Personalizado3 = request.ConfiguracionTienda.Personalizado3;
            configuracionTienda.Personalizado4 = request.ConfiguracionTienda.Personalizado4;
            configuracionTienda.RTN = request.ConfiguracionTienda.RTN;
            configuracionTienda.Telefono1 = request.ConfiguracionTienda.Telefono1;
            configuracionTienda.Telefono2 = request.ConfiguracionTienda.Telefono2;
            configuracionTienda.Telefono3 = request.ConfiguracionTienda.Telefono3;

            TransactionInfo transactionInfo2 = request.RequestUserInfo.CrearTransactionInfo("CrearCliente");
            _genericRepository.UnitOfWork.Commit(transactionInfo2);
            return "La tienda se edito";
        }
    }
}
