## Description
Calendar service, used to manage calendar events, behind mobile calendar service.

## Artifacts
### Backend service - Calendar Service
| Framework | Test Framework | Package Tool | Deployment Notes | Development Notes |
| --------- | -------------- | ------------ | ---------------- | ----------------- |
| .net8     | xunit          | dotnet cli   | in web server with IIS | try setup dev env with eng/setup-dev-env.ps1 |

### DB Migration
| Framework | Package Tool | Deployment Notes | Development Notes |
| --------- | ------------ | ---------------- | ----------------- |
| .netstandard 2.0 | dotnet cli | package with fluent migrator console runner | can be run with `migrate-calendar-db.ps1` |
