Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports EasyImGui

Public Class Program

    Public Shared Sub Main(ByVal args As String())
        Dim SingleImguiWindow As ImGuiForm = New ImGuiForm(True, True)
        AddHandler SingleImguiWindow.Render, Function()

                                                 If DearImguiSharp.ImGui.Button("Message", New DearImguiSharp.ImVec2() With {
                                                         .X = 200,
                                                         .Y = 20
                                                     }) Then
                                                     Task.Run(Sub()
                                                                  MessageBox.Show("Hello World!")
                                                              End Sub)
                                                 End If

                                                 If DearImguiSharp.ImGui.Button("Close Me", New DearImguiSharp.ImVec2() With {
                                                         .X = 200,
                                                         .Y = 20
                                                     }) Then SingleImguiWindow.Visible = False
                                                 Return True
                                             End Function

        Application.Run(SingleImguiWindow)
    End Sub

End Class
