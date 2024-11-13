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
    public class productRepository : GenericRepository<Product>, IproductRepository
    {
        private readonly ApplicationDbContext context;

        public productRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Product product)
        {
            var productInDb = context.Products.FirstOrDefault(x=>x.Id==product.Id);
            if(productInDb != null)
            {
                productInDb.Name = product.Name;
                productInDb.Description = product.Description;
                productInDb.Price = product.Price;
                productInDb.Image = product.Image;
                productInDb.CategoryId = product.CategoryId;
            }
        }
    }
}
