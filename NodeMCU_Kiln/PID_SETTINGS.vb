Public Class PID_SETTINGS
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.Close()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_PID_P_01, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown1.Value))
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_PID_I_01, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown2.Value))
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_PID_D_01, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown3.Value))

        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_PID_P_02, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown4.Value))
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_PID_I_02, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown5.Value))
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_PID_D_02, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown6.Value))

        MAIN.SendModbusCoil(MAIN.MB_CMD_WRITE_EEPROM, True)
    End Sub
End Class