@echo off
setlocal enabledelayedexpansion

set "SCRIPT_DIR=%~dp0"
set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if not exist "%CSC%" (
    echo C# compiler not found at "%CSC%"
    echo Make sure .NET Framework 4.x is installed.
    echo.
    pause
    exit /b 1
)

echo Compiling Easy Xbox Mode GUI...
echo.

"%CSC%" /nologo /target:winexe /win32manifest:"%SCRIPT_DIR%xboxmode_gui.manifest" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Core.dll "%SCRIPT_DIR%xboxmode_gui.cs"

if %errorlevel% equ 0 (
    echo.
    echo SUCCESS! Created: %SCRIPT_DIR%xboxmode_gui.exe
    echo.
    echo Run "%SCRIPT_DIR%xboxmode_gui.exe" as Administrator to use.
) else (
    echo.
    echo Compilation failed with error code %errorlevel%.
)

echo.
pause
