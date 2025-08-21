using IntiGuard.Models;
using IntiGuard.Repositories;
namespace IntiGuard.Services.Interfaces
{
    public interface IUsuarioService
    {
        void RegistrarUsuario(Usuario usuario);
        Usuario ActualizarUsuario(int id, Usuario usuario);
        IEnumerable<Usuario> ObtenerUsuarios();
        Usuario ObtenerPorId(int id);
        bool EliminarUsuario(int id);
    }
}
