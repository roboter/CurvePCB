
# CurvedPCB ✨

## First steps
![first steps](/images/58oo1h.gif)

# Goal
![make pcb with Curves](/images/goal.png)

## KiCAD example
https://mitxela.com/projects/melting_kicad

---

## Build & Run (macOS) ✅

**Quick overview:** this repository targets multiple UI backends (Avalonia, MAUI, WPF). For cross-platform development on macOS use the Avalonia Desktop project (`CurvePCB.Avalonia.UI.Desktop`).

### Prerequisites ⚙️
- **.NET SDK 8.0 or later** (check with `dotnet --version`).
  - Install via Homebrew: `brew install --cask dotnet-sdk` or from https://dotnet.microsoft.com.
- (Optional) For MAUI: install MAUI workloads and platform tooling (Xcode for iOS/Mac Catalyst, Android SDK for Android).

### Restore, build, test
1. Restore packages:

   `dotnet restore`

2. Build the whole solution:

   `dotnet build -c Release`

3. Run tests:

   `dotnet test`

### Run Avalonia Desktop (recommended on macOS) 🔧
From the repository root run:

`dotnet run --project CurvePCB.Avalonia.UI/CurvePCB.Avalonia.UI.Desktop -c Debug`

This will start the cross-platform desktop UI.

### Run Browser (WASM) 🧭
To run the browser build (serves a local site):

`dotnet run --project CurvePCB.Avalonia.UI/CurvePCB.Avalonia.UI.Browser -c Debug`

Open the URL shown in the terminal (usually http://localhost:5000).

### Windows-only (WPF) 🪟
To run the Windows WPF app (Windows only):

`dotnet run --project CurvePCB/CurvePCB.csproj`

### Troubleshooting & tips 💡
- If `dotnet` is not found or the SDK version is too old, install/upgrade the SDK and re-run `dotnet --list-sdks` to verify.
- For MAUI targets, install required workloads: `dotnet workload install maui` and follow platform-specific setup guides.
- If a project fails to run, re-run `dotnet restore` and `dotnet build -v diag` to get more details.

---

If you'd like, I can also add a short script or `Makefile` to simplify these commands. 🔧
