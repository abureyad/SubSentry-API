using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSentry.DAL.Entities
{
    public class SubscriptionTier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., Pro, Basic etc
        public decimal Price { get; set; } // FinTech rule kinda: use 'decimal' for currency
        public string Description { get; set; } = string.Empty;
    }
}