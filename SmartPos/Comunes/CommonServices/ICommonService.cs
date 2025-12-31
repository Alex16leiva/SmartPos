using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;

namespace SmartPos.Comunes.CommonServices
{
    public interface ICommonService
    {
        RequestUserInfo GetRequestInfo();

        List<PermisosDTO> ObtenerPermisos();

        void SetearRequestInfo(UsuarioDTO usuarioDTO, int caja);

        void ShowWarning(string message);

        void ShowError(string message);

        void ShowSuccess(string message);
    }
}
