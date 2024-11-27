# EasyImgui Documentation

## Introduction

**EasyImgui** is a library that allows you to integrate [ImGui](https://github.com/ocornut/imgui), a popular immediate mode graphical user interface, with WinForms applications using the .NET Framework. It provides an overlay that renders over DirectX 9 (DX9), enabling developers to create rich, interactive GUIs for their applications with ease.

![{4EE41353-4973-4B5A-ABF9-B6F4EC06A7CB}](https://github.com/user-attachments/assets/d3be3420-551e-4de4-ac21-2011ba565015)

![{E59BADD4-BFA8-48B4-B9F2-29C1E15262AF}](https://github.com/user-attachments/assets/0eb383b7-511c-463c-872e-ce50324a3f42)

## Table of Contents

1. [Features](#features)
2. [Installation](#installation)
3. [Example Usage](#example-usage)
4. [Conclusion](#conclusion)

## Features

- **Easy Integration**: Seamlessly integrate ImGui with WinForms applications.
- **Interactive Overlays**: Supports interactive overlays that can be toggled between interactive and non-interactive states.

## Installation

### Option 1: Install via NuGet Package Manager

1. **Create a .NET Framework 4.8 Project**: Open Visual Studio and create a new Console application targeting .NET Framework 4.8.

2. **Install EasyImgui via NuGet**:

   - **Package Manager Console**:
   
     Open the Package Manager Console and run:

     ```powershell
     Install-Package EasyImGui -Version 1.1.0
     ```

   - **.NET CLI**:

     If you prefer using the .NET CLI, run:

     ```bash
     dotnet add package EasyImGui --version 1.1.0
     ```

   - **Visual Studio Package Manager GUI**:

     - Right-click on your project in the Solution Explorer.
     - Select **Manage NuGet Packages**.
     - Search for **EasyImGui**.
     - Install version **1.1.0**.

### Option 2: Compile from Source

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/DestroyerDarkNess/EasyImGui.git
   ```

2. **Open the Solution**:

   Open the `EasyImGui.sln` file in Visual Studio.

3. **Restore NuGet Packages**:

   - Visual Studio should automatically restore the required NuGet packages.
   - If not, right-click on the solution and select **Restore NuGet Packages**.

4. **Build the Project**:

   Build the solution to compile the EasyImgui library/TestApp.

5. **Add Reference**:

   - In your own project, add a reference to the compiled `EasyImGui.dll`.
   - Add additional references: `RenderSpy.dll` , `Hexa.NET.ImGui.dll` , `Hexa.NET.ImGui.Backends.dll`.

## Example Usage

To start using EasyImgui in your application:

1. **Import**:

   ```csharp
   using EasyImGui;
   using EasyImGui.Core;
   using System;
   using System.Windows.Forms;
   ```

2. **Copy and Paste example code (External Normal Mode)** :

   ```csharp
     internal class Program
     {
   
         public static Overlay OverlayWindow = null;
   
         static void Main(string[] args)
         {
   
             bool result = Diagnostic.RunDiagnostic();
   
             if (result)
             {
                 Console.WriteLine("All diagnostics passed. The system is ready.");
             }
             else
             {
                 Console.WriteLine("Some diagnostics failed. Please resolve the missing libraries, Press any key to continue.");
                 Console.ReadKey();
             }
   
             OverlayWindow = new Overlay() { EnableDrag = false, ResizableBorders = true, ShowInTaskbar = true };
   
             OverlayWindow.ImguiManager.ConfigContex += OnConfigContex;
             OverlayWindow.OnImGuiReady += (object sender, bool Status) =>
             {
                 if (Status)
                 {
                     OverlayWindow.ImguiManager.Render += Render;
                 }
                 else { Console.WriteLine("Unable to initialize Imgui"); }
             };
   
             try { Application.Run(OverlayWindow); } catch (Exception Ex) { MessageBox.Show(Ex.Message); Environment.Exit(0); }
         }
   
   
         private static bool OnConfigContex()
         {
             OverlayWindow.ImguiManager.IO.ConfigDebugIsDebuggerPresent = false;
             OverlayWindow.ImguiManager.IO.ConfigErrorRecoveryEnableAssert = false;
             OverlayWindow.Location = new System.Drawing.Point(0, 0);
             OverlayWindow.Size = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
             OverlayWindow.Interactive(true);
             return true;
         }
   
         private static bool Render()
         {
             Hexa.NET.ImGui.ImGui.ShowDemoWindow();
   
             return true;
         }
   
     }
   ```

  3. **EasyImGui has 2 additional modes for overlaying on applications (InGame and InGameEmbed) if you want to see how to implement it check the complete example:** [Program.cs](https://github.com/DestroyerDarkNess/EasyImGui/blob/main/TestApp/Program.cs)

## Conclusion

EasyImgui simplifies the implementation and use of imgui, it also provides you with overlay methods for your application.... cheats.....

**Key Takeaways**:

- **Ease of Use**: Minimal setup required to get started.
- **Customizable**: Events and methods allow for extensive customization.
- **Interactive Overlays**: Easily toggle between interactive and non-interactive states.

---

This version of EasyImgui uses [Hexa.NET.ImGui](https://github.com/HexaEngine/Hexa.NET.ImGui) , credits to [JunaMeinhold](https://github.com/JunaMeinhold) , Contributions are welcome.
