# CarCare

CarCare is a Blazor web app (.NET 10, ASP.NET Core Identity, EF Core + SQLite) that helps vehicle owners keep
their vehicle's fuel and maintenance history in one place and get reminders when service is due.
CSE 325 individual project.

## User guide

1. **Register** an account (Register in the menu) and log in.
2. **Vehicle Setup**: answer the 4 steps (make and model, year, initial odometer reading, confirm).
   Each account tracks one primary vehicle.
3. **Fuel**: record each fill-up (date, odometer, gallons, total cost). You can edit or delete any record.
4. **Maintenance**: record each service (type, date, odometer, cost). Optionally set the *next service date*
   and/or *next service odometer* to get a reminder.
5. **Dashboard**: see current odometer, total fuel cost, average fuel economy (needs 2+ fill-ups),
   total maintenance cost, and maintenance reminders (Upcoming / Due soon / Overdue).

Reminders are "Due soon" within 30 days or 500 miles of the target, and "Overdue" once the date has passed or the
odometer has reached the target. The current odometer is the highest reading recorded anywhere.

## Run locally

```bash
cd CarCare
dotnet run
```

The database (`CarCare/Data/app.db`) is created and migrated automatically on first run.

## Run the tests

```bash
dotnet test CarCare.Tests
```

## Deploy

A `Dockerfile` is included. Build and run it anywhere that runs containers:

```bash
docker build -t carcare .
docker run -p 8080:8080 -v carcare-data:/data carcare
```

Mount a volume at `/data` to keep the SQLite database between restarts.

## Project layout

- `CarCare/Models` - Vehicle, FuelRecord, MaintenanceRecord
- `CarCare/Services/VehicleStats.cs` - statistics and reminder logic (unit tested)
- `CarCare/Components/Pages` - Vehicle setup, Fuel, Maintenance, Dashboard
- `CarCare.Tests` - xUnit tests

## Scope

**In:** authentication, one vehicle per account, fuel records (CRUD), maintenance records (CRUD),
dashboard statistics, maintenance reminders.

**Out:** GPS tracking, OBD-II integration, insurance, mechanic booking, multiple vehicles, AI recommendations.

## Status

- [x] Authentication
- [x] Single vehicle onboarding wizard
- [x] Fuel records
- [x] Maintenance records
- [x] Dashboard
- [x] Maintenance reminders
- [x] Unit tests
- [ ] Cloud deployment (Dockerfile ready, needs a hosting account)
