# EasyImgui Documentation

## Introduction

**EasyImgui** is a library that allows you to integrate [ImGui](https://github.com/ocornut/imgui), a popular immediate mode graphical user interface, with WinForms applications using the .NET Framework. It provides an overlay that renders over DirectX 9 (DX9), enabling developers to create rich, interactive GUIs for their applications with ease.

![image](https://github.com/user-attachments/assets/bf604c4c-0611-42ae-8f09-993f59a55c26)


---

## Table of Contents

1. [Features](#features)
2. [Installation](#installation)
3. [Getting Started](#getting-started)
4. [Class Overview](#class-overview)
   - [Overlay](#overlay-class)
   - [D3D9Window](#d3d9window-class)
   - [ImguiManager](#imguimanager-class)
5. [Example Usage](#example-usage)
6. [Conclusion](#conclusion)

---

## Features

- **Easy Integration**: Seamlessly integrate ImGui with WinForms applications.
- **Interactive Overlays**: Supports interactive overlays that can be toggled between interactive and non-interactive states.

---

## Installation

### Option 1: Install via NuGet Package Manager

1. **Create a .NET Framework 4.8 Project**: Open Visual Studio and create a new WinForms application targeting .NET Framework 4.8.

2. **Install EasyImgui via NuGet**:

   - **Package Manager Console**:
   
     Open the Package Manager Console and run:

     ```powershell
     Install-Package EasyImGui -Version 1.0.8
     ```

   - **.NET CLI**:

     If you prefer using the .NET CLI, run:

     ```bash
     dotnet add package EasyImGui --version 1.0.8
     ```

   - **Visual Studio Package Manager GUI**:

     - Right-click on your project in the Solution Explorer.
     - Select **Manage NuGet Packages**.
     - Search for **EasyImGui**.
     - Install version **1.0.8**.

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

   Build the solution to compile the EasyImgui library.

5. **Add Reference**:

   - In your own project, add a reference to the compiled `EasyImGui.dll`.
   - Add additional references: `RenderSpy.dll` , `Hexa.NET.ImGui.dll` , `Hexa.NET.ImGui.Backends.dll`.

---

## Getting Started

To start using EasyImgui in your application:

1. **Create an Instance of the Overlay**:

   ```csharp
   using EasyImGui;

   Overlay overlayWindow = new Overlay();
   ```

2. **Configure the ImGui Context** (Optional):

   ```csharp
   overlayWindow.ImguiManager.ConfigContex += () =>
   {
       // Customize ImGui style, colors, etc.
       var style = ImGui.GetStyle();
       style.WindowRounding = 5.0f;
       // Other customizations...
       return true;
   };
   ```

3. **Handle the ImGui Ready Event**:

   ```csharp
   overlayWindow.OnImGuiReady += (sender, status) =>
   {
       if (status)
       {
           // ImGui is ready; set up rendering logic
           overlayWindow.ImguiManager.Render += () =>
           {
               // Your ImGui rendering code here
               ImGui.ShowDemoWindow();
               return true;
           };
       }
   };
   ```

4. **Run the Overlay**:

   ```csharp
   Application.Run(overlayWindow);
   ```

---

## Class Overview

### Overlay Class

**Namespace**: `EasyImGui`

The `Overlay` class inherits from `D3D9Window` and serves as the main window for rendering the ImGui overlay.

#### Key Features:

- **Initialization**: Sets up the Direct3D9 device and initializes ImGui.
- **Events**:
  - `OnImGuiReady`: Triggered when ImGui is ready to render.
- **Methods**:
  - `FitTo(IntPtr windowHandle, bool attachToClientArea)`: Adapts the overlay size and position to another window.
  - `PlaceAbove(IntPtr windowHandle)`: Places the overlay above a specified window in the Z-order.
  - `Interactive(bool status)`: Toggles the overlay's interactivity.

### D3D9Window Class

**Namespace**: `RenderSpy.Overlay`

The `D3D9Window` class extends `RenderForm` and provides the underlying Direct3D9 rendering capabilities.

#### Key Features:

- **Device Management**: Handles the creation and resetting of the Direct3D9 device.
- **Rendering Loop**: Uses `RenderLoop` to continuously render frames.
- **Events**:
  - `OnD3DReady`: Triggered when the Direct3D9 device is ready.
- **Customization**:
  - Supports window dragging and resizable borders.
  - Allows for custom present parameters.

#### Usage Notes:

- This class is primarily used internally by the `Overlay` class.
- Can be extended or modified for advanced rendering scenarios.

---

### ImguiManager Class

**Namespace**: *EasyImGui.Core*

The `ImguiManager` class manages the ImGui context and rendering pipeline.

#### Key Features:

- **Initialization**: Loads ImGui bindings and initializes the context.
- **Event Handling**:
  - `Render`: Event for rendering ImGui elements.
  - `ConfigContex`: Event for configuring the ImGui context.
  - `OnLostDevice`: Handles device loss scenarios.
  - `OnResetDevice`: Handles device resets.
- **Direct3D9 Hooks**: Hooks into Direct3D9 `Present` and `Reset` methods for rendering.

## Example Usage

Below is a basic example demonstrating how to use EasyImgui to create an overlay with ImGui in a WinForms application.

```csharp
using EasyImGui;
using Hexa.NET.ImGui;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Overlay overlayWindow = new Overlay()
            {
                EnableDrag = false,
                ResizableBorders = true,
                ShowInTaskbar = false
            })
            {
                // Configure the ImGui context
                overlayWindow.ImguiManager.ConfigContex += () =>
                {
                    var style = ImGui.GetStyle();
                    style.WindowRounding = 5.0f;
                    // Other style customizations...

                    overlayWindow.Size = new Size(800, 600);
                    return true;
                };

                // Handle the ImGui ready event
                overlayWindow.OnImGuiReady += (sender, status) =>
                {
                    if (status)
                    {
                        bool showDemoWindow = true;

                        overlayWindow.ImguiManager.Render += () =>
                        {
                            // ImGui rendering code
                            ImGui.ShowDemoWindow(ref showDemoWindow);

                            // Toggle overlay interactivity based on ImGui window visibility
                            overlayWindow.Interactive(showDemoWindow);

                            return true;
                        };
                    }
                };

                // Run the overlay window
                try
                {
                    Application.Run(overlayWindow);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Environment.Exit(0);
                }
            }
        }
    }
}
```

**Explanation**:

- **Creating the Overlay**: An instance of `Overlay` is created with specific properties.
- **Configuring ImGui**: The `ConfigContex` event is used to customize the ImGui style and window size.
- **Rendering ImGui**: The `Render` event is where ImGui rendering logic is placed.
- **Running the Application**: The overlay window is run inside a `using` block to ensure proper disposal.

**see:** [TestApp/Program.cs](https://github.com/DestroyerDarkNess/EasyImGui/blob/main/TestApp/Program.cs)

---

## Conclusion

EasyImgui simplifies the implementation and use of imgui, it also provides you with overlay methods for your application.... cheats.....

**Key Takeaways**:

- **Ease of Use**: Minimal setup required to get started.
- **Customizable**: Events and methods allow for extensive customization.
- **Interactive Overlays**: Easily toggle between interactive and non-interactive states.

---

This version of EasyImgui uses [Hexa.NET.ImGui](https://github.com/HexaEngine/Hexa.NET.ImGui) , credits to [JunaMeinhold](https://github.com/JunaMeinhold) , Contributions are welcome.
