namespace IntiGuard.Services
{
    public interface ICrud<T>
    {
        T Create(T entity);
        T GetById(int id);
        IEnumerable<T> GetAll();
        T Update(int id,T entity);
        bool DeleteById(int id);
        bool ExistsById(int id);
    }
}