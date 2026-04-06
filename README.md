# Invoice API - Cloud Deployed

Welcome to the Invostaq Invoice API! This project is a fully functional, cloud-native validation API built with **.NET 8 (Isolated Worker Model)** and mapped securely to Microsoft Azure.

## 🌩️ Cloud Migration Overview
This started as a local-only SQLite application. It has now been upgraded to an auto-scaling cloud architecture:
1. **Azure SQL Server**: The API seamlessly auto-switches from SQLite to a fully hosted Azure SQL database when running in the cloud. Password injection is handled securely through Bicep and GitHub Secrets.
2. **Infrastructure as Code (IaC)**: We constructed an Infrastructure pipeline using `main.bicep`. This allows the exact cloud architecture (App Service, Storage, SQL Firewalls) to be instantiated directly from version control.
3. **CI/CD Pipeline**: Any push to the `main` branch triggers our custom `.github/workflows/deploy.yml`. It dynamically builds the .NET codebase, evaluates Bicep infrastructure, logs into Azure with a headless Service Principal, and seamlessly publishes the artifact.

## 🚀 How to Run

### 1. Running Locally (SQLite)
If `SqlConnectionString` is absent, the API safely defaults to an auto-generated SQLite database (just like Task 1).
```bash
dotnet build
func start
```
- Local Swagger UI: `http://localhost:7071/api/swagger/ui`

### 2. Running in Azure (Microsoft SQL)
When triggered via GitHub Actions, the Bicep template injects `SqlConnectionString` into the Azure Function's App Settings. Entity Framework automatically detects this and pivots to use your live Microsoft SQL production database.

**Live Cloud Swagger UI:**
👉 [https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/swagger/ui](https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/swagger/ui)
*(Note: It may take ~5 minutes for Azure to fully sync triggers and cold-start the Docker container after a fresh GitHub deployment.)*

## 🛣️ API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| **POST** | `/api/invoice` | Validates and saves a new invoice payload. Returns `201 Created` with the ID, or `400 Bad Request` if validation constraints (e.g., negative Amount, empty InvoiceNumber) are violated. |
| **GET** | `/api/invoice/{id}` | Retrieves an invoice by its exact globally unique identifier (GUID). Returns `200 OK` or `404 Not Found`. |
