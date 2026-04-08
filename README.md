# 🏦 BankingApp

> A secure RESTful banking API built with **ASP.NET Core 8**, **Clean Architecture**, and integrated **SIEM-ready structured logging** — designed as a SOC lab simulation environment.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-EF%20Core%208-336791?style=flat-square&logo=postgresql)
![JWT](https://img.shields.io/badge/Auth-JWT%20Bearer-000000?style=flat-square&logo=jsonwebtokens)
![Serilog](https://img.shields.io/badge/Logging-Serilog%20JSON-CC0000?style=flat-square)
![Swagger](https://img.shields.io/badge/Docs-Swagger%20%2F%20OpenAPI-85EA2D?style=flat-square&logo=swagger)

---

## 📌 Overview

BankingApp is a backend API simulating a real banking system, built for two purposes:

- **Functional**: A working API for user management, account operations, and OTP-based authentication.
- **Security Lab**: Structured Serilog logs feed a QRadar/Logstash SIEM pipeline to simulate and detect security events (login failures, brute force, SQL injection probes).

---

## 📐 Architecture

This project follows **Clean Architecture** with strict layer separation:

```
BankingApp/
├── BankingApp.APi              # Entry point — Controllers, DI wiring, Middleware
├── BankingApp.Application      # Service interfaces (contracts only)
├── BankingApp.Domain           # Entities & domain rules (zero external dependencies)
├── BankingApp.Infrastructure   # EF Core, PostgreSQL, service implementations
└── BankingApp.Shared           # DTOs shared across layers
```

**Dependency rule**: each layer only depends inward. Domain has no external dependencies.

---

## 🛠️ Tech Stack

| Concern        | Technology                                                   |
|----------------|--------------------------------------------------------------|
| Framework      | ASP.NET Core 8                                               |
| ORM            | Entity Framework Core 8 + Npgsql (PostgreSQL)                |
| Authentication | JWT Bearer                                                   |
| Logging / SIEM | Serilog — compact JSON rolling file + console sink           |
| API Docs       | Swagger / OpenAPI (JWT auth pre-configured)                  |
| Security       | SHA-256 OTP hashing, IP capture on all auth events           |

---

## 📦 Domain Model

| Entity        | Description                                               |
|---------------|-----------------------------------------------------------|
| `User`        | Core identity — name, email, role, phone                  |
| `Account`     | Bank account linked to a user, unique account number      |
| `Transaction` | Deposit / Withdrawal / Transfer records per account       |
| `Loan`        | Loan application with principal, rate, term, status       |
| `Otp`         | Time-limited (5 min), SHA-256 hashed one-time password    |

---

## 🔌 API Endpoints

### Auth
| Method | Endpoint              | Description                          |
|--------|-----------------------|--------------------------------------|
| POST   | `/api/auth/login`     | Authenticate and receive JWT token   |

### Users
| Method | Endpoint              | Description       |
|--------|-----------------------|-------------------|
| GET    | `/api/user/{id}`      | Get user by ID    |
| POST   | `/api/user`           | Create user       |
| PUT    | `/api/user/{id}`      | Update user       |
| DELETE | `/api/user/{id}`      | Delete user       |

### Accounts
| Method | Endpoint                 | Description          |
|--------|--------------------------|----------------------|
| GET    | `/api/account/{id}`      | Get account by ID    |
| POST   | `/api/account`           | Create account       |
| PUT    | `/api/account/{id}`      | Update account       |
| DELETE | `/api/account/{id}`      | Delete account       |

### OTP (2FA)
| Method | Endpoint                          | Description                    |
|--------|-----------------------------------|--------------------------------|
| POST   | `/api/otp/generate?userId=`       | Generate and store hashed OTP  |
| POST   | `/api/otp/validate?userId=&code=` | Validate OTP against hash      |
| POST   | `/api/otp/invalidate?userId=`     | Invalidate active OTP          |

### Security Lab (SIEM simulation)
| Method | Endpoint                           | Description                              |
|--------|------------------------------------|------------------------------------------|
| POST   | `/api/security/simulate-login`     | Simulates auth events for SIEM ingestion |
| GET    | `/api/security/simulate-attack`    | Simulates SQL injection probe log        |

---

## 🔐 Authentication

JWT Bearer is configured in `Program.cs`. Include the token in all protected requests:

```
Authorization: Bearer <your_token>
```

Swagger UI has JWT auth pre-configured — use the **Authorize** button at the top right.

---

## 📡 SIEM Integration (QRadar / Logstash)

All security events emit structured JSON logs that SIEM pipelines can ingest directly.

**Log format (compact JSON, daily rolling):**
```json
{
  "@t": "2025-12-10T14:23:01Z",
  "@l": "Warning",
  "@mt": "SECURITY_EVENT: Login Failed | User: {Username} | IP: {SrcIp} | Reason: Bad Credentials",
  "Username": "attacker",
  "SrcIp": "192.168.1.105"
}
```

**Events tracked:**

| Event                         | Level       | Trigger                        |
|-------------------------------|-------------|--------------------------------|
| Login Successful              | Information | Valid credentials              |
| Login Failed                  | Warning     | Bad credentials (SIEM alert)   |
| OTP Generated                 | Information | New OTP issued                 |
| OTP Validated                 | Information | OTP accepted                   |
| OTP Invalidated               | Warning     | OTP manually revoked           |
| SQL Injection Attempt         | Error       | `/simulate-attack` endpoint    |

Logs are written to `logs/bankapi<date>.json`.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 14+](https://www.postgresql.org/download/)

### Setup

```bash
# Clone
git clone https://github.com/simo2018000/BankingApp.git
cd BankingApp

# Restore
dotnet restore

# Set your DB credentials in BankingApp.APi/appsettings.json
# "DefaultConnection": "Server=localhost;Port=5432;Database=banking_db;User Id=postgres;Password=yourpassword;"

# Apply migrations
dotnet ef database update --project BankingApp.Infrastructure --startup-project BankingApp.APi

# Run
dotnet run --project BankingApp.APi
```

API: `http://localhost:5131` | Swagger: `http://localhost:5131/swagger`

---

## 📁 Configuration

| File                                          | Purpose                                |
|-----------------------------------------------|----------------------------------------|
| `BankingApp.APi/appsettings.json`             | DB connection string, allowed hosts    |
| `BankingApp.APi/appsettings.Development.json` | Dev log levels                         |
| `BankingApp.APi/Program.cs`                   | DI registration, Serilog, JWT, Swagger |

---

## 📄 License

MIT
