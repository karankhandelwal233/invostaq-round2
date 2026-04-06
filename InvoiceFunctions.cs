using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApi.Data;
using InvoiceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace InvoiceApi
{
    public class InvoiceFunctions
    {
        private readonly InvoiceDbContext _db;
        private readonly ILogger<InvoiceFunctions> _logger;

        public InvoiceFunctions(InvoiceDbContext db, ILogger<InvoiceFunctions> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ─── POST /api/invoice ───────────────────────────────────────────
        [Function("CreateInvoice")]
        [OpenApiOperation(operationId: "CreateInvoice", tags: new[] { "Invoice" })]
        [OpenApiRequestBody("application/json", typeof(Invoice), Description = "JSON containing customerName and amount")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Invoice), Description = "The created invoice")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Validation failed")]
        public async Task<IActionResult> CreateInvoice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invoice")]
            HttpRequest req)
        {
            _logger.LogInformation("CreateInvoice triggered.");
            try
            {
                string requestBody;
                using (var reader = new StreamReader(req.Body))
                    requestBody = await reader.ReadToEndAsync();

                Invoice? invoice;
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    invoice = JsonSerializer.Deserialize<Invoice>(requestBody, options);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("Invalid JSON: {Message}", ex.Message);
                    return new BadRequestObjectResult(new { error = "Invalid JSON format" });
                }

                if (invoice == null)
                    return new BadRequestObjectResult(new { error = "Request body is empty" });
                if (string.IsNullOrWhiteSpace(invoice.CustomerName))
                    return new BadRequestObjectResult(new { error = "CustomerName is required" });

                if (invoice.Amount <= 0)
                    return new BadRequestObjectResult(new { error = "Amount must be greater than zero" });

                invoice.Id = Guid.NewGuid();
                invoice.CreatedAt = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(invoice.Status))
                    invoice.Status = "pending";

                _db.Invoices.Add(invoice);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Invoice {Id} created.", invoice.Id);
                return new ObjectResult(invoice) { StatusCode = StatusCodes.Status201Created };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateInvoice");
                return new ObjectResult(new { error = "Internal server error" })
                    { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        // ─── GET /api/invoice/{id} ───────────────────────────────────────
        [Function("GetInvoice")]
        [OpenApiOperation(operationId: "GetInvoice", tags: new[] { "Invoice" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the invoice")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Invoice), Description = "The requested invoice")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Invoice not found")]
        public async Task<IActionResult> GetInvoice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoice/{id:guid}")]
            HttpRequest req,
            Guid id)
        {
            _logger.LogInformation("GetInvoice triggered for id {Id}.", id);
            try
            {
                var invoice = await _db.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice {Id} not found.", id);
                    return new NotFoundObjectResult(new { error = $"Invoice {id} not found" });
                }
                return new OkObjectResult(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetInvoice");
                return new ObjectResult(new { error = "Internal server error" })
                    { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
