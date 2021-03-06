﻿Imports KSPMM_Reloaded.Internal
Public Class ModIO_UC

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub ModIO_UC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Internal.ModIO.Mods = Internal.ModIO.LoadModsFromSettings()
        RebuildTree()
    End Sub
    Public Structure ObjectChildren
        Public Item As Object
        Public Children As List(Of ObjectChildren)
    End Structure
    Public Sub RebuildTree()
        TreeView1.Nodes.Clear()
        For Each m As Internal.Modification In Internal.ModIO.Mods
            Dim n As New List(Of String)
            For Each mm As Internal.ModSelection In m.ModSelections
                n.Add(mm.ModEntryName)
            Next
            Dim TN As TreeNode
            For Each nodePath As String In n
                TN = Nothing
                For Each node As String In nodePath.Split("/")
                    If node = "" Then Continue For
                    If IsNothing(TN) Then
                        If TreeView1.Nodes.ContainsKey(node) Then
                            TN = TreeView1.Nodes(node)
                        Else
                            TN = TreeView1.Nodes.Add(node, node)
                        End If
                    Else
                        If TN.Nodes.ContainsKey(node) Then
                            TN = TN.Nodes(node)
                        Else
                            TN = TN.Nodes.Add(node, node)
                        End If
                    End If
                Next
            Next
        Next
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim open As New OpenFileDialog
        open.Filter = "Compressed ZIP Folders (*.zip)|*.zip|All Files (*.*)|*.*"
        open.FileName = ""
        If open.ShowDialog() = DialogResult.Cancel Or open.FileName = "" Then Exit Sub
        Internal.AddMod(New Internal.Modification(open.FileName, Internal.Compression.Zip))
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        MsgBox("Not in alpha #2, sorry", MsgBoxStyle.Exclamation)
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        If Internal.UnloadMods() Then MsgBox("Unloading Complete!") Else MsgBox("Unloading Failed!", MsgBoxStyle.Critical)
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        If My.Settings.KSPDir = "" Then MsgBox("KSP Directory folder has not been selected.")
        ModIO.UnloadMods()
        Dim i As Integer = 0
        For Each node As TreeNode In TreeView1.Nodes
            Dim l As List(Of ModSelection) = GetChildren(node)
            Dim i1 As Integer = 0
            Dim ml As ModSelection() = ModIO.Mods(i).ModSelections.ToArray
            For Each m As ModSelection In ml
                If m.ModEntryName.EndsWith("/") Then
                    For Each g As ModSelection In l
                        If g.ModEntryName & "/" = m.ModEntryName Then
                            Dim g1 = g
                            g1.ModEntryName &= "/"
                            ModIO.Mods(i).ModSelections(i1) = g1
                        End If
                    Next
                Else
                    For Each g As ModSelection In l
                        If g.ModEntryName = m.ModEntryName Then
                            ModIO.Mods(i).ModSelections(i1) = g
                        End If
                    Next
                End If
                i1 += 1
            Next
            i += 1
        Next
        If Internal.LoadMods(My.Settings.KSPDir) Then MsgBox("Loading Complete!") Else MsgBox("Loading Failed!", MsgBoxStyle.Critical)
    End Sub

    Public Function GetChildren(parentNode As TreeNode) As List(Of ModSelection)
        Dim nodes As New List(Of ModSelection)
        GetAllChildren(parentNode, nodes)
        Return nodes
    End Function

    Private Sub GetAllChildren(parentNode As TreeNode, nodes As List(Of ModSelection))
        For Each childNode As TreeNode In parentNode.Nodes
            nodes.Add(New ModSelection(childNode.FullPath, childNode.Checked))
            GetAllChildren(childNode, nodes)
        Next
    End Sub

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        Dim fold As New FolderBrowserDialog
        fold.Description = "Find KSP root directory"
        If fold.ShowDialog() = DialogResult.Cancel Then Exit Sub
        My.Settings.KSPDir = fold.SelectedPath
        My.Settings.Save()
    End Sub
End Class
