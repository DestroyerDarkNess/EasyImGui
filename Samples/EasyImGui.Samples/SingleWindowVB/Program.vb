Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DearImguiSharp
Imports EasyImGui
Imports EasyImGui.Core
Imports SharpDX.Direct3D9

Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim OverlayWindow As Overlay = New Overlay() With {
            .EnableDrag = True,
            .ResizableBorders = True,
            .Fix_WM_NCLBUTTONDBLCLK = True
        }

        AddHandler OverlayWindow.OnImGuiReady, Sub(sender As Object, Status As Boolean)
                                                   If Status Then
                                                       AddHandler OverlayWindow.Imgui.ConfigContex, Function()
                                                                                                        Dim style = ImGui.GetStyle()
                                                                                                        Dim styleColors = style.Colors
                                                                                                        styleColors(CInt(ImGuiCol.CheckMark)).W = 1.0F
                                                                                                        styleColors(CInt(ImGuiCol.CheckMark)).X = 1.0F
                                                                                                        styleColors(CInt(ImGuiCol.CheckMark)).Y = 1.0F
                                                                                                        styleColors(CInt(ImGuiCol.CheckMark)).Z = 1.0F

                                                                                                        styleColors(CInt(ImGuiCol.FrameBg)).W = 0.0F
                                                                                                        styleColors(CInt(ImGuiCol.FrameBg)).X = 0.0F
                                                                                                        styleColors(CInt(ImGuiCol.FrameBg)).Y = 0.0F
                                                                                                        styleColors(CInt(ImGuiCol.FrameBg)).Z = 0.0F

                                                                                                        style.WindowRounding = 5.0F
                                                                                                        style.FrameRounding = 5.0F
                                                                                                        style.FrameBorderSize = 1.0F

                                                                                                        ' OverlayWindow.Imgui.IO.ConfigFlags += ImGuiConfigFlags.ViewportsEnable

                                                                                                        Return True
                                                                                                    End Function

                                                       Dim DrawImguiMenu As Boolean = True

                                                       Dim device As Device = OverlayWindow.D3DDevice

                                                       AddHandler OverlayWindow.Imgui.Render, Function()
                                                                                                  If Not DrawImguiMenu Then OverlayWindow.Close() : Return False

                                                                                                  If OverlayWindow.Imgui.Imgui_Ini AndAlso OverlayWindow.Imgui.IO IsNot Nothing Then

                                                                                                      Dim IsFocusOnMainImguiWindow As Boolean = (Form.ActiveForm Is OverlayWindow)
                                                                                                      If IsFocusOnMainImguiWindow Then InputHook.Universal(OverlayWindow.Imgui.IO)
                                                                                                      OverlayWindow.EnableDrag = If(DearImguiSharp.ImGui.IsAnyItemActive(), False, IsFocusOnMainImguiWindow)


                                                                                                      ImGui.SetNextWindowPos(New ImVec2 With {.X = 0, .Y = 0}, 0, New ImVec2 With {.X = 0, .Y = 0})
                                                                                                      ImGui.SetNextWindowSize(New ImVec2 With {.X = OverlayWindow.ClientSize.Width, .Y = OverlayWindow.ClientSize.Height}, 0)

                                                                                                      ImGui.Begin(OverlayWindow.Text, DrawImguiMenu, 0)

                                                                                                      If ImGui.Button("Message", New ImVec2 With {.X = OverlayWindow.ClientSize.Width - 15, .Y = 20}) Then
                                                                                                          Task.Run(Sub()
                                                                                                                       MessageBox.Show("Hello World!")
                                                                                                                   End Sub)
                                                                                                      End If

                                                                                                      If ImGui.Button("Exit", New ImVec2 With {.X = OverlayWindow.ClientSize.Width - 15, .Y = 20}) Then
                                                                                                          OverlayWindow.Close()
                                                                                                      End If
                                                                                                  End If
                                                                                                  Return True
                                                                                              End Function
                                                   End If
                                               End Sub

        Try : Application.Run(OverlayWindow) : Catch ex As Exception : Environment.Exit(0) : End Try
    End Sub

End Class
