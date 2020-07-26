Public Class SCHEDULE_NAME
    Private Sub SCHEDULE_NAME_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' prevent resize
        Me.MinimumSize = Me.Size
        Me.MaximumSize = Me.Size
        TextBox1.MaxLength = MAIN.MAX_STRING_LENGTH
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        While TextBox1.Text.Length < MAIN.MAX_STRING_LENGTH
            TextBox1.Text = TextBox1.Text & " "
        End While
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_SCH_NAME, EasyModbus.ModbusClient.ConvertStringToRegisters(TextBox1.Text))
        Me.Close()
    End Sub
End Class