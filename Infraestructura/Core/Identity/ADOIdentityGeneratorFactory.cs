using CrossCutting.Identity;
using Infraestructura.Core;
using Microsoft.Extensions.Options;

namespace CrossCutting.Network.Identity
{
    public class ADOIdentityGeneratorFactory : IIdentityFactory
    {
        private readonly string _connectionString;

        public ADOIdentityGeneratorFactory(IOptions<DataBaseSetting> options)
        {
            _connectionString = options.Value.conectionDataBase;
        }

        public IIdentityGenerator Create()
        {
            return new ADOIdentityGenerator(_connectionString);
        }
    }

}