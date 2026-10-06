# OrdersApp

Small .NET console app that processes `orders.json` (take-home assignment).

## Commands
- Build: `dotnet build`
- Test: `dotnet test`
- Run: `dotnet run --project src/OrdersApp`

## Structure
- `src/OrdersApp` – console app
- `tests/OrdersApp.Tests` – xUnit tests
- `_specs/` – feature specs

## Conventions
- Keep it simple: no layered architecture, no MediatR/CQRS, no DI container.
- Business logic lives in a separate service with pure methods
  (takes `IReadOnlyList<Order>`, no file I/O), so it is testable without files.
- Use `decimal` for money.
- Read the data file via `AppContext.BaseDirectory`, path overridable by CLI argument.
- No extra NuGet packages beyond what the templates include.

## Working rules
- Specs live in `_specs/`. Implement only against an approved spec; update the spec first if requirements change.
- Propose a plan before writing code; wait for approval.
- Never change expected values in existing tests to make them pass — ask instead.
- Keep changes small and focused.