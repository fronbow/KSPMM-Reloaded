﻿Public Class Network_UC
    Public Sub AddDownload(ByVal Download As Internal.Download, UC As Panel)
        Dim d As New NetworkDownload_UC(Download)
        d.DName.Text = d.Download.Location.Remove(0, d.Download.Location.LastIndexOf("/"c) + 1)
        d.DPriority.Text = [Enum].GetName(GetType(Internal.Priority), d.Download.Priority)
        d.DImage.Image = Icon.ExtractAssociatedIcon(d.Download.Location).ToBitmap
        AddHandler d.Download.StatusChange, AddressOf d.ChangeStatus
        AddHandler d.Download.ProgressUpdate, AddressOf d.UpdateProgress
        UC.Controls.Add(d)
        d.Dock = DockStyle.Top
        d.SendToBack()
        Internal.Network.DownloadReady(Download)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim save As New SaveFileDialog
        save.Filter = "All Files (*.*)|*.*"
        save.FileName = TextBox1.Text.Remove(0, TextBox1.Text.LastIndexOf("/"c) + 1)
        If save.ShowDialog() = DialogResult.Cancel Or save.FileName = "" Then Exit Sub
        Dim down As New Internal.Download(New Uri(TextBox1.Text), save.FileName, Internal.Priority.Normal)
        Internal.Network.Add(down)
    End Sub
End Class
