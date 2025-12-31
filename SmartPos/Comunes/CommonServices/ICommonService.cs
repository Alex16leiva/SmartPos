using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;

namespace SmartPos.Comunes.CommonServices
{
    public interface ICommonService
    {
        RequestUserInfo GetRequestInfo();

        List<PermisosDTO> ObtenerPermisos();

        void SetearRequestInfo(UsuarioDTO usuarioDTO, int caja);

        void ShowSuccess(string title, string message);

        void ShowWarning(string title, string message);

        void ShowError(string title, string message);
    }
}
