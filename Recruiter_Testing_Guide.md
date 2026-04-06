# Invostaq Cloud Environment - Testing Guide

The Invoice API has been successfully migrated to a serverless Azure Cloud environment and securely connected to a Microsoft SQL database. 

*(Note: During cloud deployment, the built-in Swagger UI was disabled as per production security best practices, but the underlying API endpoints and database are 100% functional and ready for evaluation).*

Here is exactly how to test the active cloud infrastructure from your local machine.

## 1. Create a New Invoice (POST)

You can use Postman, Insomnia, or your terminal to ping the active Azure Functions endpoint. 

**Endpoint:** `https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/invoice`
**Method:** `POST`
**Headers:** `Content-Type: application/json`

**cURL Command:**
```bash
curl -X POST https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/invoice \
     -H "Content-Type: application/json" \
     -d '{"CustomerName":"Recruiter Test Account", "Amount":550}'
```

**Expected Response (201 Created):**
Because the API contains rigid validation rules, passing valid data will return a `201 Created` payload alongside a newly generated Azure SQL GUID:
```json
{
  "id": "a12a391b-811a-44f8-92c9-52bb69c54927",
  "customerName": "Recruiter Test Account",
  "amount": 550,
  "status": "pending",
  "createdAt": "2026-04-06T12:03:39.5546776Z"
}
```

---

## 2. Retrieve an Invoice (GET)

Once you generate an invoice, copy the `id` from your response body and append it to the endpoint URL to fetch the persistent data back from the Microsoft SQL Server.

**Endpoint:** `https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/invoice/{Your-Generated-ID}`
**Method:** `GET`

For instance, you can click the example link below in your web browser right now to fetch the test invoice I previously generated on the cloud:
👉 [https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/invoice/a12a391b-811a-44f8-92c9-52bb69c54927](https://invostaq4y3gemhjmn4ns-func.azurewebsites.net/api/invoice/a12a391b-811a-44f8-92c9-52bb69c54927)

---

## 3. Verify Edge Cases (Validation Rules)

The API actively validates incoming requests. You can intentionally break the rules to verify the logic:

* **Negative Amounts:** Try passing `"Amount": -100`. The server will reject the SQL write and return `400 Bad Request` with `"error": "Amount must be greater than zero"`.
* **Missing Name:** Try omitting `"CustomerName"`. The server will return `400 Bad Request`.
* **Invalid ID's:** Try fetching a fake ID (e.g., `/api/invoice/00000000-0000-0000-0000-000000000000`). The server will return `404 Not Found`.
