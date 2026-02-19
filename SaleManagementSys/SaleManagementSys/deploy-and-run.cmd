@echo off
setlocal
cd /d "%~dp0"

echo Building and publishing...
dotnet publish -c Release -o ".\publish" --nologo
if errorlevel 1 (
    echo Publish failed.
    exit /b 1
)

echo.
echo Starting app (Production, listening on all interfaces :5000)...
echo.
echo  - On this PC:     http://localhost:5000
echo  - On your LAN:   http://YOUR_LOCAL_IP:5000
echo  - From internet: http://YOUR_PUBLIC_IP:5000  (after port forwarding)
echo.
echo To allow access: Windows Firewall ^> Allow an app ^> allow port 5000 (or run this script as Admin once).
echo To get your local IP: run   ipconfig   and look for IPv4 Address.
echo.
echo Press Ctrl+C to stop the server.
echo ----------------------------------------

set ASPNETCORE_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://0.0.0.0:5000
dotnet ".\publish\SaleManagementSys.dll"
