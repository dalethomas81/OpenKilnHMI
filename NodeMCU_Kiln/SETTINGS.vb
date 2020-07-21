Public Class SETTINGS
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.Close()
    End Sub

    Private Sub SETTINGS_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' prevent resize
        Me.MinimumSize = Me.Size
        Me.MaximumSize = Me.Size
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        MAIN.SendModbusCoil(MAIN.MB_CMD_THERM_OVERRIDE, CheckBox1.Checked)
    End Sub
End Class