# CurvedPCB

## Project Overview
CurvedPCB is a Windows application for creating PCB designs with curved connections. The project consists of multiple components working together to provide a complete solution for PCB design with curved routing capabilities.

## Project Structure
The solution contains the following main projects:

- **CurvePCB** - Main WPF desktop application that serves as the primary user interface
- **CurvePCB.Lib** - Core library containing shared logic, data models and utilities
- **CurvePCB.Test** - Unit tests for the core functionality
- **CurvePCB.Ui** - User interface components
- **CurvePCB.Maui** - .NET MAUI mobile application (cross-platform)
- **CurvePCB.Avalonia.UI** - Avalonia-based cross-platform UI implementation
- **PathfindingWithBezier** - Pathfinding algorithms with Bezier curve support

## System Requirements

### Prerequisites
- Windows 10 or higher (Windows 7.0 target framework)
- .NET 8.0 SDK or later
- Visual Studio 2022 or later (recommended)

### Development Environment
- Windows 10 or higher
- Visual Studio 2022 with .NET desktop development workload
- .NET 8.0 SDK
- WPF and Avalonia UI development tools

### Optional Platforms
- For cross-platform support:
  - .NET MAUI development tools (Visual Studio 2022 with .NET MAUI workload)
  - Avalonia UI development tools

## Building the Project

### Prerequisites
1. Install .NET 8.0 SDK from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Install Visual Studio 2022 with .NET desktop development workload
3. Ensure all required NuGet packages are restored (automatically handled by Visual Studio)

### Build Steps
1. Open `CurvePCB.sln` in Visual Studio
2. Restore NuGet packages if needed:
   ```
   dotnet restore
   ```
3. Build the solution:
   ```
   dotnet build
   ```

### Building for Release
To build a release version:
```
dotnet build -c Release
```

## Running the Application

### From Visual Studio
1. Open `CurvePCB.sln` in Visual Studio
2. Set the startup project to `CurvePCB`
3. Press F5 or click "Start Debugging" to run the application

### From Command Line
1. Navigate to the main directory:
   ```
   cd CurvePCB
   ```
2. Run the application directly:
   ```
   dotnet run --project CurvePCB\CurvePCB.csproj
   ```

## Project Dependencies

### NuGet Packages
- Svg 3.4.4 - SVG rendering support
- Microsoft.NET.Sdk - .NET SDK for building applications

### Core Libraries
- CurvePCB.Lib - Shared logic and data models
- PathfindingWithBezier - Bezier curve pathfinding algorithms

## Features and Goals

This project aims to create PCB designs with curved connections, inspired by:
- Traditional PCB design tools
- KiCAD and other open-source PCB design software
- Bezier curve pathfinding algorithms for optimal routing
- Curved connection visualization and manipulation

The main goal is to demonstrate how curved paths can be used in PCB design to potentially improve signal integrity or reduce electromagnetic interference.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests to ensure nothing is broken
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Inspired by KiCAD and PCB design tools
- Bezier curve pathfinding algorithms
- WPF and Avalonia UI frameworks

## First Steps
![first steps](/images/58oo1h.gif)

## Goal
![make pcb with Curves](/images/goal.png)

## KiCAD example
https://mitxela.com/projects/melting_kicad
