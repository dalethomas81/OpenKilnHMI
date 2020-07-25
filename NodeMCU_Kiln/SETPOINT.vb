Public Class SETPOINT
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_CMD_SETPOINT, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown1.Value))
        Me.Close()
    End Sub
End Class