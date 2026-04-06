# Invostaq Cloud Assessment: Invoice API

This repository contains the complete, production-ready solution for the backend cloud assessment. The objective was to evaluate fundamentals in .NET, Azure Functions, Entity Framework Core, as well as cloud infrastructure design (IaC) and CI/CD automation pipelines.

---

## 🧪 Live Cloud Evaluation

The Azure infrastructure is currently deeply integrated with a live Microsoft SQL Database and running on a serverless Consumption Plan. 

> [!IMPORTANT]  
> **Testing the Live Environment:** As per production security best practices, the visual Swagger UI endpoint is hidden on the public Azure deployment. 
> 
> Please read the **[Recruiter Testing Guide (Recruiter_Testing_Guide.md)](./Recruiter_Testing_Guide.md)** located in the root of this repository. It contains exact `curl` payloads and live Azure endpoints showcasing how to interact with the active cloud environment.

---

## 🎯 Task 1: API Development (Local Execution)

Built a RESTful API using the **.NET 8 Isolated Worker Model** running safely on an Azure Functions HTTP trigger context.

### Endpoints
The backend conforms strictly to REST principles, integrating extensive local `OpenAPI` coverage to simplify endpoint discovery:
- **`POST /api/invoice`** 
  - **Purpose:** Validates and persists an incoming invoice JSON payload.
  - **Validation:** Implements strict data checks (e.g., rejecting negative billing amounts, enforcing non-empty customer strings).
  - **Returns:** `201 Created` with a freshly generated immutable GUID, or `400 Bad Request` citing the specific validation failure.
- **`GET /api/invoice/{id}`** 
  - **Purpose:** Retrieves the invoice data associated with a parsed GUID.
  - **Returns:** `200 OK` alongside the retrieved JSON payload, or `404 Not Found` if the GUID does not exist.

### Data Storage Strategy
- **Framework:** Developed using **Entity Framework (EF) Core**.
- **Dynamic Database Routing:** 
  - **Local Development:** When executed locally via `func start`, the API detects the absence of Azure environmental settings and dynamically falls back to an automatically generated SQLite database (`invoice.db`). This allows code reviewers to rigorously test the API immediately out of the box without requiring a full SQL Server installation.
  - **Cloud Production:** In Azure, the environment is cleanly injected via Bicep context, pivoting the `DbContext` provider to connect safely to a persistent Microsoft SQL Server.

---

## 🌩️ Task 2: Infrastructure & CI/CD Automation

Migrated the isolated local solution into an automated, elastically scalable cloud architecture utilizing Microsoft Azure.

### Infrastructure as Code (IaC)
Authored strictly declarative infrastructural templates utilizing **Azure Bicep** (`infra/main.bicep`). Bicep was explicitly chosen over manual portal configuration to ensure perfect environment repeatability. The script auto-provisions:
1. **Azure SQL Server & Database:** Hardened via internal Azure IP firewall bindings.
2. **Azure Function App:** Configured heavily for `.NET 8 Isolated`. The application is hosted atop an elastic Consumption plan maximizing serverless cost-efficiency. *(Note: During the build, the region target was intelligently overridden to `centralus` to successfully bypass severe Microsoft Free-Tier `eastus` quota limitations without blocking deployment).*
3. **Azure Storage Account:** Automatically attached to securely manage the internal Function App trigger state.

### Automation / deployment pipeline
An intricate CI/CD workflow (`.github/workflows/deploy.yml`) actively watches the `main` git branch to orchestrate automated releases.
- **Authentication Stage:** Authenticates headless against Azure using an isolated Service Principal token injected purely via GitHub Action Secrets.
- **Build Stage:** Compiles the .NET 8 codebase into a production-agnostic deployment artifact.
- **Provision Stage:** Validates and executes the Bicep template. This cleanly links the database by securely passing the `SQL_ADMIN_PASSWORD` secret straight from GitHub into the Azure App Settings, entirely eliminating hard-coded connection strings.
- **Deployment Stage:** Pushes the finalized, built codebase smoothly onto the running serverless layout.

---

## 📦 Deliverables Checklist
- [x] **Source code in GitHub:** Verified
- [x] **IaC templates:** Located under `/infra`
- [x] **Workflow YAML:** Located under `/.github/workflows/deploy.yml`
- [x] **README documentation:** (This document)

---

## 🚀 How to Run Locally

Because the backend natively injects a SQLite database when a remote cloud server is not accessible, local testing mimics the production experience natively:

1. Clone the repository to your desktop.
2. Navigate into the active folder and trigger the initial build:
   ```bash
   dotnet build
   ```
3. Boot the local Function App host:
   ```bash
   func start
   ```
4. A local database (`invoice.db`) will auto-compile in your folder.
5. Navigate to the fully generated documentation and interact with the endpoints safely via the web UI:
   - 👉 `http://localhost:7071/api/swagger/ui`
