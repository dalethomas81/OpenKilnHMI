Public Class TUNING
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        PID_SETTINGS.NumericUpDown1.Value = MAIN.Kiln_01.TemperatureController.Upper.P
        PID_SETTINGS.NumericUpDown2.Value = MAIN.Kiln_01.TemperatureController.Upper.I
        PID_SETTINGS.NumericUpDown3.Value = MAIN.Kiln_01.TemperatureController.Upper.D

        PID_SETTINGS.NumericUpDown4.Value = MAIN.Kiln_01.TemperatureController.Lower.P
        PID_SETTINGS.NumericUpDown5.Value = MAIN.Kiln_01.TemperatureController.Lower.I
        PID_SETTINGS.NumericUpDown6.Value = MAIN.Kiln_01.TemperatureController.Lower.D
        PID_SETTINGS.Show()
    End Sub

    ' set upper high
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_CAL_TEMP_ACT_CH0, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown1.Value))
        MAIN.SendModbusCoil(MAIN.MB_CMD_CAL_CH0_HIGH, True)
        MAIN.SendModbusCoil(MAIN.MB_CMD_WRITE_EEPROM, True)
    End Sub

    ' set upper low
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_CAL_TEMP_ACT_CH0, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown1.Value))
        MAIN.SendModbusCoil(MAIN.MB_CMD_CAL_CH0_LOW, True)
        MAIN.SendModbusCoil(MAIN.MB_CMD_WRITE_EEPROM, True)
    End Sub

    ' set lower high
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_CAL_TEMP_ACT_CH1, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown2.Value))
        MAIN.SendModbusCoil(MAIN.MB_CMD_CAL_CH1_HIGH, True)
        MAIN.SendModbusCoil(MAIN.MB_CMD_WRITE_EEPROM, True)
    End Sub

    ' set lower low
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        MAIN.SendModbusMultipleHoldingRegisters(MAIN.MB_CAL_TEMP_ACT_CH1, EasyModbus.ModbusClient.ConvertFloatToRegisters(NumericUpDown2.Value))
        MAIN.SendModbusCoil(MAIN.MB_CMD_CAL_CH1_LOW, True)
        MAIN.SendModbusCoil(MAIN.MB_CMD_WRITE_EEPROM, True)
    End Sub

End Class