using System;

namespace InvoiceApi.Models
{
    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string CustomerName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Status { get; set; } = "pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
