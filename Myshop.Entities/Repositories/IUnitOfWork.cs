using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myshop.Entities.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IproductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IApplicationUserRepository ApplicationUser { get; }
        int Complete();
    }
}
