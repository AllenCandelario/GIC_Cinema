# GIC Cinemas – Booking System  
A cinema booking prototype implemented with **.NET 9** and **React (Vite + TypeScript)**.  
This project demonstrates clean layering and domain-driven design practices, with both a **Console (CLI)** and a **Web (React + .NET API)** presentation layer.

---

## 🧩 Overview

The assignment required a console-based seat booking application.  
I implemented the **CLI** version first and later expanded it into a **Web API + React** application, keeping the same shared business logic (`GIC_Cinema.Core`).  
This allows both the **CLI** and **Web API + React** to call into the same core domain and application layers.

---

## 🧱 Architecture

### High-Level
The solution follows a **2-layer architecture**:

| Layer | Description |
|-------|--------------|
| **Presentation Layer** | Two separate frontends: `GIC_Cinema.CLI` (Console) and `GIC_Cinema.Web` + `gic-cinema-ui` (React). They handle input/output and user interaction only. |
| **Business/Core Layer** | `GIC_Cinema.Core`: Contains all application, domain, and shared logic. This ensures domain rules are isolated and reusable. |

This structure reflects **Clean Architecture principles** by separating the **UI** (outer layer) from the **domain and application logic** (inner layer).

### Internal Organization

**GIC_Cinema**
- **GIC_Cinema.Core** → .NET 9 Class library containing Domain logic & entities
    - `Application/`
      - `Services`/ → Application layer (orchestration)
    - `Domain/`
      - `Entities/` → Core domain models (CinemaMovie, Booking, Seat)
      - `Services/` → Domain logic (SeatAllocator)
    - `Shared/` → Static helpers (Validation, BookingIdGenerator)
- **GIC_Cinema.CLI** → Console UI (menu + cinema layout rendering)
- **GIC_Cinema.Web** → Minimal .NET 9 Web API endpoints
- **GIC_Cinema.Tests** → xUnit tests
- **gic-cinema-ui** → React TypeScript frontend (Vite)



---

## 🧰 Prerequisites

### For CLI version
- [.NET SDK 9.0+](https://dotnet.microsoft.com/)

### For Web version
- [.NET SDK 9.0+](https://dotnet.microsoft.com/)
- [Node.js (Latest LTS)](https://nodejs.org/)

---

## 🚀 Running the Application

### Option 1 – Console (CLI)
 
**1. Start the Console (CLI):**
   - Open a terminal at the project root.
   - cd GIC_Cinema.CLI
   - dotnet run

### Option 2 – Web API + React
**1. Start the Web API**
   - Open a terminal at the project root.
   - cd GIC_Cinema.Web
   - dotnet run
   - **Change the localhost ports in /Properties/launchSettings.json if you have to**
    
**2. Start the React frontend**
   - cd ../gic-cinema-ui
   - npm install (only for the first time)
   - npm dev run
   - **If you encounter execution policy errors, run Get-ExecutionPolicy (to check) and then Set-ExecutionPolicy RemoteSigned**
   - **Change the endpoint ports in vite.config.ts if you have to**

---

## 🧪 Testing 
Unit tests (xUnit) are provided in the GIC_Cinema.Tests project. Run with dotnet test
