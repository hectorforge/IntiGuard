using IntiGuard.Models;

namespace IntiGuard.Repositories.Interfaces
{
    public interface IUsuarioCrud : ICrud<Usuario>
    {
        void CreateWithTransaction(Usuario usuario);
        Usuario UpdateWithTransaction(int id, Usuario usuario);
        bool HasPurchases(int id);
    }
}
