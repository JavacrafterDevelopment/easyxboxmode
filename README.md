This app is completely Vibe Coded so use at own risk. The Xbox Mode GUI App can be used for easy enabling of both the xbox mode via vivetool and also the launch at startup using Registry Keys.

Everything below that is written is also vibecoded so idk it may or may not be right.



# Xbox Mode

A command-line tool to enable Xbox-related features on Windows desktops.

## What it does

- **Xbox Mode** — Uses `ViVeTool` to enable hidden Xbox feature IDs, giving your desktop Xbox capabilities like the Launch on Startup home-app picker.
- **Handheld Mode** — Sets the `DeviceForm` registry value to `0x2e`, making Windows treat your desktop as a handheld device. This unblocks features Microsoft restricts to handhelds only.

## Requirements

- Windows (run Command Prompt as **Administrator**)
- All files must stay together in the same folder

## Usage

Open Command Prompt as Administrator, `cd` to the folder with the files, and run:

```
xboxmode help
```

### Commands

| Command | Description |
|---|---|
| `xboxmode help` | Show available commands |
| `xboxmode enable` | Enable Xbox feature IDs via ViVeTool |
| `xboxmode disable` | Disable Xbox feature IDs via ViVeTool |
| `xboxmode status` | Show ViVeTool status, device type, and summary |
| `xboxmode handheldmode enable` | Set `DeviceForm` to `0x2e` (handheld mode) |
| `xboxmode handheldmode disable` | Remove the `DeviceForm` registry value |
| `xboxmode handheldmode status` | Show handheld mode status only |

## Files

| File | Purpose |
|---|---|
| `xboxmode.bat` | The main CLI wrapper |
| `ViVeTool.exe` | Tool for enabling/disabling Windows feature IDs |
| `Albacore.ViVe.dll` | Dependency for ViVeTool |
| `Newtonsoft.Json.dll` | Dependency for ViVeTool |
| `FeatureDictionary.pfs` | Feature ID lookup data for ViVeTool |

## Registry

Handheld mode modifies:

```
HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\OEM
    DeviceForm = 0x2e (DWORD)
```

`xboxmode handheldmode disable` deletes the `DeviceForm` value entirely (Windows does not create this key by default, so there is nothing to restore).
