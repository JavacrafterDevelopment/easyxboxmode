@echo off
setlocal enabledelayedexpansion

set "VIVETOOL=%~dp0ViVeTool.exe"

if "%1"=="" goto help

if /i "%1"=="help" goto help
if /i "%1"=="enable" goto enable
if /i "%1"=="disable" goto disable
if /i "%1"=="status" goto status
if /i "%1"=="handheldmode" goto handheldmode

echo Unknown command: %1
echo.
goto help

:help
echo Xbox Mode - Easy Xbox feature toggling for Windows
echo.
echo Usage:  xboxmode ^<command^> [args]
echo.
echo Commands:
echo   help                         Show this help message
echo   enable                       Enable Xbox Mode features
echo   disable                      Disable Xbox Mode features
echo   status                       Show current Xbox Mode status
echo   handheldmode enable          Enable handheld mode (DeviceForm reg)
echo   handheldmode disable         Disable handheld mode (remove reg entry)
echo   handheldmode status          Show handheld mode status only
echo.
goto end

:enable
echo Enabling Xbox Mode features...
"%VIVETOOL%" /enable /id:59765208,58989070
echo.
goto end

:disable
echo Disabling Xbox Mode features...
"%VIVETOOL%" /disable /id:59765208,58989070
echo.
goto end

:status
echo --- ViVeTool Feature Status ---
"%VIVETOOL%" /query /id:59765208
"%VIVETOOL%" /query /id:58989070
echo.
echo --- Device Type ---
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM" /v DeviceForm 2>nul >nul
if !errorlevel! equ 0 (
    reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM" /v DeviceForm 2>nul | find /i "0x2e" >nul
    if !errorlevel! equ 0 (
        echo Device is set to Handheld Mode (DeviceForm = 0x2e)
    ) else (
        echo DeviceForm is set to a non-handheld value.
    )
) else (
    echo Device is in Desktop Mode (DeviceForm value not present)
)
echo.
echo --- Summary ---
echo Xbox Mode: Enabled when feature IDs 59765208 and 58989070 show "Enabled"
echo Handheld Mode: Enabled when DeviceForm = 0x2e
echo.
goto end

:handheldmode
if "%2"=="" goto handheldmode_help
if /i "%2"=="enable" goto handheldmode_enable
if /i "%2"=="disable" goto handheldmode_disable
if /i "%2"=="status" goto handheldmode_status
echo Unknown handheldmode command: %2
goto handheldmode_help

:handheldmode_help
echo Handheld Mode - Fools Windows into thinking this is a handheld device
echo.
echo Usage:  xboxmode handheldmode ^<command^>
echo.
echo Commands:
echo   enable    Set DeviceForm to 0x2e (handheld mode)
echo   disable   Remove DeviceForm registry value
echo   status    Show current handheld mode status
echo.
goto end

:handheldmode_status
echo --- Handheld Mode Status ---
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM" /v DeviceForm 2>nul >nul
if !errorlevel! equ 0 (
    reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM" /v DeviceForm 2>nul | find /i "0x2e" >nul
    if !errorlevel! equ 0 (
        echo Handheld Mode is ENABLED (DeviceForm = 0x2e)
    ) else (
        echo Handheld Mode is set to a non-handheld value.
    )
) else (
    echo Handheld Mode is DISABLED (DeviceForm value not present)
)
echo.
goto end

:handheldmode_enable
echo Enabling handheld mode...
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM" /v DeviceForm /t REG_DWORD /d 0x2e /f
echo.
goto end

:handheldmode_disable
echo Disabling handheld mode...
reg delete "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM" /v DeviceForm /f 2>nul
if !errorlevel! equ 0 (
    echo DeviceForm registry value removed.
) else (
    echo DeviceForm registry value not found or already removed.
)
echo.
goto end

:end
endlocal
