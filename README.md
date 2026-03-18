# Survey Basket API 🧺

<p align="center">
  <strong>A structured ASP.NET Core Web API for building, publishing, and analyzing surveys and polls.</strong>
</p>

<p align="center">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-10.0-blueviolet" />
  <img alt="API" src="https://img.shields.io/badge/API-ASP.NET%20Core-5C2D91" />
  <img alt="Database" src="https://img.shields.io/badge/Database-SQL%20Server-CC2927" />
  <img alt="Auth" src="https://img.shields.io/badge/Auth-JWT-success" />
  <img alt="Jobs" src="https://img.shields.io/badge/Jobs-Hangfire-orange" />
</p>

---

## 📚 Table of Contents

- [Project Overview](#-project-overview)
- [Core Features Implemented](#-core-features-implemented)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Application Flow](#-application-flow)
- [How to Run the Project](#-how-to-run-the-project)
- [Configuration Checklist](#-configuration-checklist)
- [API Modules](#-api-modules)
- [Highlights](#-highlights)

---

## 🎯 Project Overview

**Survey Basket API** is a backend application for managing the full survey lifecycle:

- user registration and login,
- email confirmation and password reset,
- poll creation and publishing,
- question and answer management,
- secure vote submission,
- result aggregation and reporting,
- scheduled notifications and health monitoring.

This project is designed as a **feature-rich survey platform API** with a focus on clean separation of responsibilities, validation, authentication, observability, and scalability.

---

## ✅ Core Features Implemented

### 1) Authentication & Account Management
- User registration with ASP.NET Core Identity.
- JWT-based authentication.
- Refresh token issuance and revocation.
- Email confirmation workflow.
- Forgot password and reset password flow.
- Authenticated profile retrieval and profile update.
- Password change for logged-in users.
- Rate limiting on authentication endpoints.

### 2) Poll Management
- Create, update, retrieve, and delete polls.
- Get all polls.
- Get currently active published polls.
- Toggle poll publish status.
- Prevent duplicate poll titles.
- Automatically trigger notifications when a poll is published on the current date.

### 3) Question & Answer Management
- Add questions under a specific poll.
- Retrieve poll questions with pagination.
- Search questions by content.
- Sort questions by content.
- Update question content and answer set.
- Toggle question active/inactive status.
- Prevent duplicate questions per poll.

### 4) Voting System
- Start a voting session by loading available questions and active answers.
- Allow authenticated users to vote on active published polls.
- Prevent duplicate votes by the same user.
- Validate that submitted answers match the poll's available questions.
- Apply concurrency rate limiting for vote submission.
- Cache available questions for faster vote startup.

### 5) Results & Analytics
- Retrieve raw poll vote data.
- Get vote counts grouped by day.
- Get vote counts grouped by question and answer.
- Support reporting-ready endpoints for dashboards and analytics.

### 6) Background Processing & Notifications
- Hangfire background job integration.
- Daily recurring notification job for newly available polls.
- Email notifications for poll publishing.
- Background email sending for account confirmation.

### 7) Reliability, Validation & Monitoring
- FluentValidation request validation.
- Global exception handling.
- Problem Details error responses.
- Health checks for database and Hangfire.
- Serilog structured logging.
- Swagger/OpenAPI documentation in development.
- CORS enabled for client integration.

---

## 🛠 Technology Stack

| Area | Technology |
|---|---|
| Backend | ASP.NET Core Web API |
| Framework Target | .NET 10 |
| Authentication | ASP.NET Core Identity + JWT |
| Database | SQL Server + Entity Framework Core |
| Background Jobs | Hangfire |
| Validation | FluentValidation |
| Object Mapping | Mapster |
| Logging | Serilog |
| Email | MailKit |
| API Docs | Swagger / Swashbuckle |
| Monitoring | Health Checks |
| Caching | Hybrid Cache + Distributed Memory Cache |
| Security | Rate Limiting + Authorization |

---

## 🗂 Project Structure

```text
Survey-Basket/
├── SurveyBasketAPI/
│   ├── Controllers/           # API endpoints
│   ├── Services/              # Business logic
│   ├── Services Abstraction/  # Service contracts
│   ├── DTOs/                  # Request/response models
│   ├── Entities/              # Domain entities
│   ├── Persistence/           # DbContext, configurations, migrations
│   ├── Validations/           # FluentValidation validators
│   ├── Templates/             # Email HTML templates
│   ├── Result Pattern/        # Result and error abstraction
│   ├── Common/                # Shared helpers, filters, constants
│   ├── Health/                # Health check implementations
│   └── Program.cs             # Application bootstrap
└── README.md
```

---

## 🔄 Application Flow

<details>
  <summary><strong>Click to expand a simplified workflow</strong></summary>

1. **User registers** → confirmation email is generated.
2. **User confirms email** → account becomes ready for login.
3. **User logs in** → receives JWT access token and refresh token.
4. **Admin or authorized operator creates polls** → adds questions and answers.
5. **Poll is published** → notification job may send alert emails.
6. **Authenticated users start voting** → active questions are loaded from cache/database.
7. **Users submit votes** → the system validates poll state and duplicate voting rules.
8. **Results endpoints aggregate data** → clients can display analytics and reports.

</details>

---

## 🚀 How to Run the Project

### Prerequisites
Make sure you have the following installed:

- .NET SDK 10.0
- SQL Server
- A mail provider account or SMTP sandbox (for email features)
- Optional: Visual Studio / VS Code

### 1. Clone the repository
```bash
git clone <your-repository-url>
cd Survey-Basket
```

### 2. Restore dependencies
```bash
dotnet restore SurveyBasket.slnx
```

### 3. Configure the database connection strings
Update `SurveyBasketAPI/appsettings.json` or use user secrets/environment variables for:

- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:HangfireConnection`

### 4. Configure JWT settings
Provide values for:

- `JWT:Key`
- `JWT:Issuer`
- `JWT:Audience`
- `JWT:ExpiryMinutes`

> Recommended: keep secrets out of `appsettings.json` and use **User Secrets** or environment variables instead.

### 5. Configure email settings
Provide values for:

- `MailSettings:Mail`
- `MailSettings:Password`
- `MailSettings:Host`
- `MailSettings:Port`
- `MailSettings:DisplayName`

### 6. Configure Hangfire dashboard credentials
Provide values for:

- `HangfireSettings:UserName`
- `HangfireSettings:Password`

### 7. Apply database migrations
```bash
dotnet ef database update --project SurveyBasketAPI
```

> If Entity Framework CLI tools are not installed, run:
```bash
dotnet tool install --global dotnet-ef
```

### 8. Run the API
```bash
dotnet run --project SurveyBasketAPI
```

### 9. Open development tools
When running in development, you can use:

- **Swagger UI**: `https://localhost:<port>/swagger`
- **Health Check**: `https://localhost:<port>/health`
- **Hangfire Dashboard**: `https://localhost:<port>/jobs`

---

## ☑️ Configuration Checklist

Use this checklist before starting the app:

- [ ] SQL Server is running.
- [ ] `DefaultConnection` is valid.
- [ ] `HangfireConnection` is valid.
- [ ] JWT key is configured.
- [ ] SMTP credentials are configured.
- [ ] Hangfire dashboard username/password are configured.
- [ ] Database migrations are applied.
- [ ] The app is started in Development mode if you want Swagger and Hangfire UI.

---

## 🔌 API Modules

### Authentication
- `POST /Auth/register`
- `POST /Auth/confirm-email`
- `POST /Auth/resend-confirmation-email`
- `POST /Auth/forget-password`
- `POST /Auth/reset-password`
- `POST /Auth`
- `POST /Auth/refresh`
- `PUT /Auth/revoke-refresh-token`

### Account
- `GET /AccountInfo`
- `PUT /AccountInfo`
- `PUT /AccountInfo/ChangePassword`

### Polls
- `GET /api/polls`
- `GET /api/polls/current`
- `GET /api/polls/{id}`
- `POST /api/polls`
- `PUT /api/polls/{id}`
- `DELETE /api/polls/{id}`
- `PUT /api/polls/{id}/togglePublish`

### Questions
- `GET /api/polls/{pollId}/questions`
- `POST /api/polls/{pollId}/questions`
- `GET /api/polls/{pollId}/questions/{questionId}`
- `PUT /api/polls/{pollId}/questions/{id}`
- `PUT /api/polls/{pollId}/questions/{questionId}/toggleStatus`

### Voting
- `GET /api/polls/{pollId}/vote`
- `POST /api/polls/{pollId}/vote`

### Results
- `GET /api/polls/{pollId}/results/row-data`
- `GET /api/polls/{pollId}/results/votes-per-day`
- `GET /api/polls/{pollId}/results/votes-per-question`

---

## 🌟 Highlights

<details>
  <summary><strong>Why this project is strong for a portfolio or production starter</strong></summary>

- It goes beyond CRUD by including **authentication, email flows, rate limiting, caching, background jobs, and analytics**.
- It uses a **service-based architecture** that keeps controllers lightweight.
- It includes **operational features** like logging, health checks, Swagger, and Hangfire.
- It demonstrates practical backend concerns such as **refresh tokens**, **duplicate vote protection**, and **result aggregation**.
- It is a strong foundation for adding:
  - admin roles,
  - dashboard UI,
  - poll categories,
  - public/private poll modes,
  - export to CSV/PDF,
  - richer analytics.

</details>

---

## ▶️ Quick Start Commands

```bash
dotnet restore SurveyBasket.slnx
dotnet ef database update --project SurveyBasketAPI
dotnet run --project SurveyBasketAPI
```

---

If you want, I can also create a **more polished README version with architecture diagrams, setup screenshots, badges, and sample request/response payloads**.
