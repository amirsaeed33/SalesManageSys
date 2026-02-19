@echo off
title Ngrok - Sale Management (port 5000)
echo.
echo Stopping any existing ngrok tunnel...
taskkill /IM ngrok.exe /F >nul 2>&1
timeout /t 2 /nobreak >nul
echo.
echo Step 1: Make sure the app is running first.
echo        Open another window and run: deploy-and-run.cmd
echo.
echo Step 2: This window will run ngrok. Keep it open.
echo        Your public URL will appear below (e.g. https://xxxx.ngrok-free.dev)
echo.
echo If you see "err_ngrok_108" or auth error, sign up at https://ngrok.com
echo then run: ngrok config add-authtoken YOUR_TOKEN
echo.
echo ----------------------------------------
ngrok http 5000
echo.
echo ----------------------------------------
echo Ngrok stopped. Press any key to close this window.
pause >nul
