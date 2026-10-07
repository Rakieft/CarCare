# CarCare

CarCare is a Blazor web app (.NET 10, ASP.NET Core Identity, EF Core + SQLite) that helps vehicle owners keep their
vehicle's fuel, driving and maintenance history in one place. CSE 325 individual project.

## Quick start

```bash
cd CarCare
dotnet run
```

Open the printed URL, register an account, then go to **Vehicle Setup** to register your vehicle.
The database (`CarCare/Data/app.db`) is created automatically on first run.

## Scope (solo version)

**In**
- Register / log in (ASP.NET Core Identity)
- Vehicle setup wizard (one primary vehicle per account)
- Fuel records (CRUD)
- Maintenance records (CRUD)
- Dashboard with basic statistics (mileage, fuel cost, consumption)
- Maintenance reminders

**Out:** GPS tracking, OBD-II integration, insurance, mechanic booking, multiple vehicles, AI recommendations (stretch).

## Status

- [x] Authentication
- [x] Single vehicle onboarding wizard
- [ ] Fuel records
- [ ] Maintenance records
- [ ] Dashboard
- [ ] Maintenance reminders
- [ ] Accessibility / Lighthouse pass, tests
- [ ] Cloud deployment
