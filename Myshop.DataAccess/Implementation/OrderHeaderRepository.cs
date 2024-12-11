using Microsoft.EntityFrameworkCore;
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
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext context;

		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(OrderHeader orderHeader)
		{
			context.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string OrderStatus, string ? PaymentStatus)
		{
			var orderfromdb = context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderfromdb != null)
			{
				orderfromdb.OrderStatus = OrderStatus;
				orderfromdb.PaymentDate = DateTime.Now;
				if (PaymentStatus != null)
				{
					orderfromdb.PaymentStatus = PaymentStatus;
				}
			}
		}
	}
}
