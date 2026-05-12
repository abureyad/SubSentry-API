using Microsoft.EntityFrameworkCore;
using SubSentry.BLL.Interfaces;
using SubSentry.DAL.Data;
using SubSentry.DAL.Entities;

namespace SubSentry.BLL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AppDbContext _context;

        public SubscriptionService(AppDbContext context)
        {
            _context = context;
        }

        // CRUD
        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Tier)
                .Include(s => s.User)
                .ToListAsync();
        }

        // Feature 1: Workflow Automation & Feature 5: Audit Logging
        public async Task<bool> ProcessRecurringBillingAsync(int subscriptionId)
        {
            var sub = await _context.Subscriptions.FindAsync(subscriptionId);
            if (sub == null) return false;

            var oldDate = sub.NextBillingDate.ToString("yyyy-MM-dd");
            sub.NextBillingDate = sub.NextBillingDate.AddMonths(1);
            var newDate = sub.NextBillingDate.ToString("yyyy-MM-dd");

            // If they were overdue, successfully billing them makes them Active again!
            if (sub.Status == "Overdue")
            {
                sub.Status = "Active";
            }

            // Audit Log
            var log = new AuditLog
            {
                SubscriptionId = sub.Id,
                Action = "Monthly Billing Cycle Update",
                OldValue = oldDate,
                NewValue = newDate
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return true;
        }

        // Feature 2: Analytics
        public async Task<decimal> GetTotalMonthlyRevenueAsync()
        {
            return await _context.Subscriptions
                .Where(s => s.Status == "Active")
                .SumAsync(s => s.Tier!.Price);
        }

        // Feature 3: Advanced Filtering
        public async Task<IEnumerable<Subscription>> SearchSubscriptionsAsync(string? status, decimal? minPrice)
        {
            var query = _context.Subscriptions
                .Include(s => s.Tier)
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(s => s.Tier != null && s.Tier.Price >= minPrice.Value);
            }

            return await query.ToListAsync();
        }

        // Feature 4: Risk Alerts
        public async Task<IEnumerable<Subscription>> GetOverdueRiskAlertsAsync()
        {
            var today = DateTime.UtcNow;
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Tier)
                .Where(s => s.NextBillingDate < today && s.Status != "Cancelled")
                .ToListAsync();
        }

        // Feature 5 Endpoint: Get History
        public async Task<IEnumerable<AuditLog>> GetSubscriptionHistoryAsync(int subscriptionId)
        {
            return await _context.AuditLogs
                .Where(a => a.SubscriptionId == subscriptionId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}