using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSentry.DAL.Entities;

namespace SubSentry.BLL.Interfaces
{
    public interface ISubscriptionService
    {
        // CRUD
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();

        // Feature 1: Workflow Automation
        Task<bool> ProcessRecurringBillingAsync(int subscriptionId);

        // Feature 2: Analytics
        Task<decimal> GetTotalMonthlyRevenueAsync();

        // Feature 3: Advanced Filtering
        Task<IEnumerable<Subscription>> SearchSubscriptionsAsync(string? status, decimal? minPrice);

        // Feature 4: Risk Alerts (This is the one you need!)
        Task<IEnumerable<Subscription>> GetOverdueRiskAlertsAsync();

        // Feature 5: Audit Logging
        Task<IEnumerable<AuditLog>> GetSubscriptionHistoryAsync(int subscriptionId);
    }
}