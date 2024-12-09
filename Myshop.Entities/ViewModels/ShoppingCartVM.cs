using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myshop.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartsList { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; } = new();
    }
}
