using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private ApplicationDbContext context;

        public EFOrderRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Order> Orders => context.Orders.Include(o => o.Lines).ThenInclude(x => x.Product);

        public void SaveOrder(Order order)
        {
            context.AttachRange(order.Lines.Select(l => l.Product));
            if(order.OrderId == 0)
            {
                context.Orders.Add(order);
            }
            context.SaveChanges();
        }
    }
}
