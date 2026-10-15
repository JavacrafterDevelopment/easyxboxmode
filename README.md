The Xbox Mode GUI App can be used for easy enabling of both the xbox mode via vivetool and also the launch at startup using Registry Keys.

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

## License

Licensed under either of

- Apache License, Version 2.0 ([LICENSE-APACHE](LICENSE-APACHE) or https://www.apache.org/licenses/LICENSE-2.0)
- MIT license ([LICENSE-MIT](LICENSE-MIT) or https://opensource.org/licenses/MIT)

at your option.

### Contribution

Unless you explicitly state otherwise, any contribution intentionally submitted
for inclusion in the work by you, as defined in the Apache-2.0 license, shall be
dual licensed as above, without any additional terms or conditions.

### Third-party components

The license above covers this project's own files: `xboxmode.bat`,
`xboxmode_gui.cs`, `xboxmode_gui.manifest`, `compile_gui.bat` and the built
`xboxmode_gui.exe`. These bundled files are someone else's and keep their own
licenses:

| Files | From | Version | License |
|---|---|---|---|
| `ViVeTool.exe`, `Albacore.ViVe.dll`, `FeatureDictionary.pfs` | [ViVeTool](https://github.com/thebookisclosed/ViVe) by thebookisclosed | [v0.3.4](https://github.com/thebookisclosed/ViVe/releases/tag/v0.3.4) ([source for this version](https://github.com/thebookisclosed/ViVe/tree/v0.3.4)) | GPL-3.0 ([text](LICENSE-GPL-3.0-ViVeTool.txt)) |
| `Newtonsoft.Json.dll` | [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) by James Newton-King | [13.0.3](https://github.com/JamesNK/Newtonsoft.Json/releases/tag/13.0.3) | MIT |

`ViVeTool.exe`, `Albacore.ViVe.dll` and `Newtonsoft.Json.dll` are byte-for-byte the
files in ViVeTool's official v0.3.4 download. `FeatureDictionary.pfs`, ViVeTool's
dictionary of feature names, is a different copy from the one in that download;
ViVeTool fetches newer dictionaries itself with `/dictupdate`.
