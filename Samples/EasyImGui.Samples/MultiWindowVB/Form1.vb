Imports EasyImGui

Public Class Form1

    Public ShowImguiDemo As Boolean = False
    Private MultiForm As MultiImGuiForm = Nothing

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MultiForm = New MultiImGuiForm()
        AddHandler MultiForm.Render, Function()
                                         If Not MultiForm.Draw Then Return False

                                         DearImguiSharp.ImGui.Begin("Another Window in C#", MultiForm.Draw, CInt(DearImguiSharp.ImGuiWindowFlags.None))
                                         DearImguiSharp.ImGui.Text("Hello from another window!")

                                         If DearImguiSharp.ImGui.Button("ImguiDemo", New DearImguiSharp.ImVec2() With {
                                             .X = 200,
                                             .Y = 20
                                         }) Then ShowImguiDemo = Not ShowImguiDemo

                                         If ShowImguiDemo Then DearImguiSharp.ImGui.ShowDemoWindow(ShowImguiDemo)

                                         DearImguiSharp.ImGui.[End]()
                                         Return True
                                     End Function

        MultiForm.Show()
    End Sub

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        If MultiForm IsNot Nothing Then MultiForm.Draw = True
    End Sub

    Private Sub button2_Click(sender As Object, e As EventArgs) Handles button2.Click
        If MultiForm IsNot Nothing Then MultiForm.Draw = False
    End Sub

End Class
