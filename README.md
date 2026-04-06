# Invostaq Cloud Assessment: Invoice API

**Live Azure Cloud Endpoint Evaluation:**
*(Note: Swagger UI is hidden in production by the OpenAPI package for security restrictions, but the live serverless API and Database are 100% online.)*

You can evaluate the functionality of the Live Azure Function natively using `curl` or Postman via the raw endpoints:

**Create an Invoice (POST):**
```bash
curl -X POST https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/invoice \
     -H "Content-Type: application/json" \
     -d '{"CustomerName":"Task 2 Cloud Test", "Amount":2500}'
```
This repository contains the complete solution for the backend cloud assessment. The objective was to evaluate fundamentals in .NET, Azure Functions, Entity Framework Core, as well as cloud infrastructure design (IaC) and CI/CD pipelines.

---

## 🎯 Task 1: API Development (Local Execution)

Built a RESTful API using the **.NET 8 Isolated Worker Model** running on an Azure Functions HTTP trigger context.

### Endpoints
The following endpoints were designed adhering to REST principles and configured with OpenAPI (Swagger) for easy testing:
- **`POST /api/invoice`** 
  - **Purpose:** Validates and saves an incoming invoice JSON payload.
  - **Validation:** Implemented strict input checks (e.g., rejecting negative amounts, enforcing required fields).
  - **Returns:** `201 Created` with the newly assigned unique ID, or `400 Bad Request` if validation fails.
- **`GET /api/invoice/{id}`** 
  - **Purpose:** Retrieves the invoice details based on its GUID.
  - **Returns:** `200 OK` with the invoice payload, or `404 Not Found` if the ID does not exist in the database.

### Data Storage Strategy
- **Framework:** Utilized **Entity Framework (EF) Core**.
- **Dynamic Database Routing:** 
  - For standard local execution (`dotnet build` / `func start`), the application natively defaults to a rapid **SQLite file database** seamlessly created on startup. This allows reviewers to run and evaluate the code offline immediately without installing SQL Server.
  - In Production (Azure), Bicep injects a `SqlConnectionString` setting causing the `DbContext` provider to gracefully pivot to a live **Microsoft SQL Server**.

---

## 🌩️ Task 2: Infrastructure & CI/CD Automation

Migrated the local solution into a fully cloud-native, scalable architecture using Azure and GitHub Actions.

### Infrastructure as Code (IaC)
Authored declarative IaC templates using **Azure Bicep** (`infra/main.bicep` and `infra/main.parameters.json`). Bicep was chosen for its native integration with the Azure Resource Manager. The template automatically provisions:
1. **Azure SQL Server & Database:** Designed with secure firewall bindings allowing only internal Azure traffic.
2. **Azure Function App:** Configured to run on deeply integrated Linux/Windows Consumption plans (`Y1`), maximizing cost-efficiency via serverless pay-per-execution. *(Note: Overrode the server location targeting `centralus` to bypass aggressive Free-Tier `eastus` quota limits).*
3. **Azure Storage Account:** Acts as the required fast-state datastore for internal Functions trigger leases.

### Automation / deployment pipeline
A complete CI/CD workflow (`.github/workflows/deploy.yml`) is triggered on every push to the `main` branch.
- **Authentication:** Injects a secure headless Azure Service Principal via GitHub Secrets (`AZURE_CREDENTIALS`).
- **Build Stage:** Restores, builds, and publishes the .NET 8 codebase into a production-ready package.
- **Provision Stage:** Compiles the Bicep templates and ensures the live Azure platform reflects the exact state written in source control. Bicep securely injects the `SQL_ADMIN_PASSWORD` (from GitHub Secrets) securely into the Azure Function's environmental memory.
- **Deploy Stage:** Syncs the built codebase payload straight onto the active serverless infrastructure.

---

## 📦 Deliverables Checklist
- [x] **Source code in GitHub:** (Complete)
- [x] **IaC templates:** Located under `/infra`
- [x] **Workflow YAML:** Located under `/.github/workflows/deploy.yml`
- [x] **README documentation:** (This document)

---

## 🚀 How to Run Locally

Because the `DbContext` falls back to SQLite, running locally is incredibly straightforward:

1. Clone the repository: `git clone https://github.com/karankhandelwal233/invostaq-round2.git`
2. Navigate into the folder and build:
   ```bash
   dotnet build
   ```
3. Start the function app:
   ```bash
   func start
   ```
4. A local `invoice.db` file will automatically compile.
5. Open the Local Swagger Explorer in your browser: `http://localhost:7071/api/swagger/ui`
