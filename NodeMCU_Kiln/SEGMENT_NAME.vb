Public Class SEGMENT_NAME
    Private Sub SEGMENT_NAME_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.MaxLength = MAIN.MAX_STRING_LENGTH
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        EasyModbus.ModbusClient.ConvertStringToRegisters(TextBox1.Text)
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_SCH_SEG_NAME, EasyModbus.ModbusClient.ConvertStringToRegisters(TextBox1.Text))
        Me.Close()
    End Sub
End Class