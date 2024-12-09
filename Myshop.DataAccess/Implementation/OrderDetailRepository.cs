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
	internal class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
	{
		private readonly ApplicationDbContext context;

		public OrderDetailRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(OrderDetail orderDetail)
		{
			context.OrderDetails.Update(orderDetail);
		}
	}
}
