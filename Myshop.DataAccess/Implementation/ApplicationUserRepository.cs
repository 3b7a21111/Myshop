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
	public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
	{
		private readonly ApplicationDbContext context;

		public ApplicationUserRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}
	}
}
