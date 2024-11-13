using Myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myshop.Entities.Repositories
{
    public interface IproductRepository :IGenericRepository<Product>
    {
        void Update(Product product);
    }
}
