using Myshop.DataAccess.Data;
using Myshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myshop.DataAccess.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public ICategoryRepository Category { get; private set; }

        public IproductRepository Product { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

		public IOrderDetailRepository OrderDetail {  get; private set; }

		public IOrderHeaderRepository OrderHeader {  get; private set; }

		public IApplicationUserRepository ApplicationUser {  get; private set; }

		public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            Category = new CategoryRepository(context);
            Product = new productRepository(context);
			ShoppingCart = new ShoppingCartRepository(context);
            OrderDetail = new OrderDetailRepository(context);
            OrderHeader = new OrderHeaderRepository(context);
            ApplicationUser = new ApplicationUserRepository(context);
		}
        public int Complete()
        {
            return context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
