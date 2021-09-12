
rem dotnet ef database update 0 -c ApplicationDbContext  -p NetConnMon.Persistence
rem dotnet ef migrations remove -c ApplicationDbContext  -p NetConnMon.Persistence
rem dotnet ef migrations add InitialCreateIdentity -c ApplicationDbContext -p NetConnMon.Persistence
rem dotnet ef database update -c ApplicationDbContext -p NetConnMon.Persistence
dotnet ef migrations add InitialCreateTestDb -c ApplicationDbContext -p NetConnMon.Persistence

dotnet ef database update 0 -c TestDbContext  -p NetConnMon.Persistence

rem dotnet ef migrations remove -c TestDbContext  -p NetConnMon.Persistence

dotnet ef migrations add InitialCreateTestDb -c ApplicationDbContext -p NetConnMon.Persistence
dotnet ef database update -c TestDbContext -p NetConnMon.Persistence -s NetConnMon 

dotnet ef migrations add -c ApplicationDbContext -p NetConnMon.Persistence -s NetConnMon CreateAspNetIdentityStuff
dotnet ef database update -c ApplicationDbContext -p NetConnMon.Persistence -s NetConnMon 

rem dotnet ef migrations add -c TestDbContext -p NetConnMon.Persistence -s NetConnMon --no-build "anything2"
rem Done. To undo this action, use 'ef migrations remove'
rem dotnet ef database update -c TestDbContext -p NetConnMon.Persistence -s NetConnMon 

rem #cd ./NetConnMon ; dotnet watch run debug
