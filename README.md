## Task 1: REST API (Local Execution)

### Prerequisites
- .NET 8 SDK
- Azure Functions Core Tools v4 (`func --version` should show 4.x)
- dotnet-ef tools (`dotnet ef --version` should show 8.x)

### Run locally

```bash
git clone https://github.com/YOUR_USERNAME/invostaq-round2.git
cd InvoiceApi
dotnet ef database update
func start
```

### Endpoints

#### POST /api/invoice
Creates a new invoice.

Request body:
```json
{
  "customerName": "Acme Corp",
  "amount": 250.00,
  "status": "pending"
}
```

Response (201 Created):
```json
{
  "id": "a1b2c3d4-...",
  "customerName": "Acme Corp",
  "amount": 250.00,
  "status": "pending",
  "createdAt": "2025-01-15T10:30:00Z"
}
```

#### GET /api/invoice/{id}
Retrieves an invoice by its GUID.

Response (200 OK or 404 Not Found)

### Tech stack
- .NET 8 isolated worker Azure Functions
- Entity Framework Core 8 with SQLite
- System.Text.Json for serialization
