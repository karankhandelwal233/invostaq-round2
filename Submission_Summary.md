# Invostaq Assignment Submission

**GitHub Repository:** [https://github.com/karankhandelwal233/invostaq-round2](https://github.com/karankhandelwal233/invostaq-round2)
**Live Azure Swagger UI:** `[Cloud deployment URL restricted for privacy]`

Below is a breakdown of how I approached and solved both tasks for the assessment.

### Task 1: API Development (.NET 8 & EF Core)
I built the API using the .NET 8 Isolated Worker model. I chose Entity Framework Core for data access because it gave me a clean way to switch between local development and cloud production. 
- **Local Dev:** When running locally (without an Azure SQL string), the app automatically sets up a local SQLite database (`invoice.db`). This makes cloning and testing the repo very fast.
- **Validation:** I implemented the `POST /api/invoice` and `GET /api/invoice/{id}` endpoints with explicit validation checks. For example, it returns a `400 Bad Request` if the amount is less than zero, or if required fields like the invoice number are missing. Successful creations return a `201 Created` with the generated ID.
- **Testing UI:** I integrated the OpenAPI extension to expose a local and live Swagger UI to make testing the endpoints easier.

### Task 2: Infrastructure as Code & CI/CD
For the cloud deployment, I wanted to avoid manual portal configurations, so I used Azure Bicep and GitHub Actions.
- **Bicep Templates:** I wrote `infra/main.bicep` to declaratively provision the Azure SQL Server, the Database, the Storage Account, and the Function App (using a serverless Consumption plan).
- **Overcoming Quota Limits:** Because my Azure account was a brand new free-tier subscription, I hit a "0 Dynamic VMs" quota limit in the `eastus` region. To solve this, I decoupled the region config and forced the Function App to provision in `centralus` to bypass the restriction.
- **GitHub Actions (CI/CD):** I set up `.github/workflows/deploy.yml`. When code is pushed to `main`, the pipeline authenticates into Azure using a Service Principal, builds the .NET artifact, deploys the Bicep infrastructure, and publishes the code automatically. The connection string is injected directly from Bicep into the App Settings, so the codebase never hardcodes credentials.

Let me know if you need any additional details on the implementation.
