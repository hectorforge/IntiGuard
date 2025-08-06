using IntiGuard.Models;
using IntiGuard.Services;

namespace IntiGuard.Repositories
{
    public class ProductoServiceImpl : IProductoService
    {

        private readonly string _connectionString;

        public ProductoServiceImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Producto Create(Producto entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Producto> GetAll()
        {
            throw new NotImplementedException();
        }

        public Producto GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Producto Update(int id, Producto entity)
        {
            throw new NotImplementedException();
        }
    }
}
