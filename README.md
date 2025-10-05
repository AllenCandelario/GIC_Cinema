# GIC Cinemas – Console Prototype
A small console application for booking cinema seats. It demonstrates clean separation between **Presentation** (CLI) and **Core** (business logic), with unit tests (xUnit) 

---

## Prerequisites
- **.NET SDK 9.0+**  

---

## Project Structure
/GIC_Cinemas
│
├─ Cinema.Core                  # Business logic
│  ├─ Application/
│  │  └─ Services/              # Application logic (orchestration)
│  │     └─ BookingService.cs
│  ├─ Domain/
│  │  ├─ Entities/
│  │  │  ├─ CinemaMovie.cs
│  │  │  ├─ Booking.cs
│  │  └─ Services/              # Domain logic
│  │     └─ SeatAllocator.cs
│  └─ Shared/                   # Shared static helper methods
│     ├─ Validation.cs          
│     └─ BookingIdGenerator.cs
│
├─ Cinema.CLI                   # Presentation (Console)
│  ├─ Program.cs                # Menu + flow (no domain logic)
│  ├─ CinemaRender.cs           # Renders seating map
│
└─ Cinema.Tests                 # xUnit tests
   ├─ ValidationTests.cs
   └─ SeatAllocatorTests.cs
