using Microsoft.AspNetCore.Mvc;
using SubSentry.BLL.Interfaces;

namespace SubSentry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Ensures automatic 400 errors and JSON binding
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        // Dependency Injection: Injecting the interface, not the concrete class 
        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet] // Standard GET method 
        public async Task<IActionResult> GetSubscriptions()
        {
            var subs = await _subscriptionService.GetAllSubscriptionsAsync();
            return Ok(subs); // Returns 200 OK with JSON
        }

        // Feature 1: Billing Workflow
        [HttpPost("{id}/process-billing")]
        public async Task<IActionResult> ProcessBilling(int id)
        {
            var result = await _subscriptionService.ProcessRecurringBillingAsync(id);
            if (!result) return NotFound("Subscription not found.");

            return Ok(new { message = "Billing processed successfully." });
        }

        // Feature 2: Financial Analytics
        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetRevenueReport()
        {
            var total = await _subscriptionService.GetTotalMonthlyRevenueAsync();
            return Ok(new { MonthlyRecurringRevenue = total });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? status, [FromQuery] decimal? minPrice)
        {
            var results = await _subscriptionService.SearchSubscriptionsAsync(status, minPrice);
            return Ok(results); // Returns JSON response
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(int id)
        {
            var history = await _subscriptionService.GetSubscriptionHistoryAsync(id);
            return Ok(history);
        }

        // Beyond-CRUD Feature 4: Financial Analytics and risk alert
        [HttpGet("alerts/overdue")]
        public async Task<IActionResult> GetOverdueAlerts()
        {
            var alerts = await _subscriptionService.GetOverdueRiskAlertsAsync();
            return Ok(new
            {
                AlertCount = alerts.Count(),
                RiskyAccounts = alerts
            });
        }
    }
 }