using Myshop.DataAccess.Data;
using Myshop.Entities.Models;
using Myshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myshop.DataAccess.Implementation
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart> , IShoppingCartRepository
	{
        private readonly ApplicationDbContext context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public int DecreaseCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncreaseCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}
