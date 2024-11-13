using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myshop.Entities.ViewModels
{
	public class ProductVM
	{
		public Product Product {  get; set; }
		[ValidateNever]
		public IEnumerable <SelectListItem>  CategoryIds { get; set; }
	}
}
