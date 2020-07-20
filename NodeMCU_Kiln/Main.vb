Public Class Main


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim mb As New EasyModbus.ModbusClient("COM26", 502)
        mb.Baudrate = 115200
        mb.Parity = IO.Ports.Parity.None
        mb.StopBits = IO.Ports.StopBits.One

        mb.Connect()

        If mb.Connected Then
            Dim qty As Int16 = 2
            Dim TempInt32(qty) As Int32
            TempInt32 = mb.ReadInputRegisters(0, qty)

            Dim TempFloat = EasyModbus.ModbusClient.ConvertRegistersToDouble(TempInt32)
            Label1.Text = TempFloat.ToString
        End If

        mb.Disconnect()
    End Sub

End Class