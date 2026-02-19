@echo off
:: Run this once as Administrator to allow inbound connections on port 5000
netsh advfirewall firewall add rule name="SaleManagementSys HTTP" dir=in action=allow protocol=TCP localport=5000
if errorlevel 1 (
    echo Failed. Make sure you run this script as Administrator.
) else (
    echo Firewall rule added: port 5000 is now allowed for inbound TCP.
)
