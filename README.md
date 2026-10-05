\# 🌾 RaithuNestham



\### AI-Powered Agricultural Assistance Platform



RaithuNestham is a full-stack agricultural assistance platform designed to help farmers access useful farming information through a single application.



The platform provides weather information, crop guidance, fertilizer and pesticide information, government schemes, equipment subsidies, and an AI-powered farming assistant supporting Telugu and English.



\---



\## 🚀 Project Overview



RaithuNestham was built as a full-stack application using:



\- Angular

\- ASP.NET Core .NET 8

\- C#

\- Entity Framework Core

\- SQL Server

\- JWT Authentication

\- RESTful Web APIs

\- Gemini AI



The application follows a layered architecture with a separate Angular frontend and ASP.NET Core backend.



\### Architecture



```text

&#x20;                   ┌──────────────────────┐

&#x20;                   │       Farmer         │

&#x20;                   │   Web Application    │

&#x20;                   └──────────┬───────────┘

&#x20;                              │

&#x20;                              ▼

&#x20;                   ┌──────────────────────┐

&#x20;                   │   Angular Frontend   │

&#x20;                   │   TypeScript / HTML  │

&#x20;                   │        / CSS         │

&#x20;                   └──────────┬───────────┘

&#x20;                              │

&#x20;                        HTTP / REST API

&#x20;                              │

&#x20;                              ▼

&#x20;                   ┌──────────────────────┐

&#x20;                   │ ASP.NET Core .NET 8  │

&#x20;                   │      Web API         │

&#x20;                   └──────────┬───────────┘

&#x20;                              │

&#x20;             ┌────────────────┼────────────────┐

&#x20;             │                │                │

&#x20;             ▼                ▼                ▼

&#x20;      ┌────────────┐   ┌────────────┐   ┌────────────┐

&#x20;      │ SQL Server │   │ JWT Auth   │   │ Gemini AI  │

&#x20;      │ + EF Core  │   │ Security   │   │ Assistant  │

&#x20;      └────────────┘   └────────────┘   └────────────┘

