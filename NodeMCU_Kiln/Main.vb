Public Class Main
    Dim CoilsCount As Int16 = 10
    Dim InputStatusCount As Int16 = 10
    Dim HoldingRegistersCount As Int16 = 50
    Dim InputRegistersCount As Int16 = 50

    Dim CoilsIn(CoilsCount) As Boolean
    Dim CoilsOut(CoilsCount) As Boolean
    Dim InputStatusIn(InputStatusCount) As Boolean
    Dim InputStatusOut(InputStatusCount) As Boolean
    Dim HoldingRegistersIn(HoldingRegistersCount) As Integer
    Dim HoldingRegistersOut(HoldingRegistersCount) As Integer
    Dim InputRegistersIn(InputRegistersCount) As Integer
    Dim InputRegistersOut(InputRegistersCount) As Integer

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim mb As New EasyModbus.ModbusClient("COM26")
        mb.Baudrate = 115200
        mb.Parity = IO.Ports.Parity.None
        mb.UnitIdentifier = 1
        mb.StopBits = IO.Ports.StopBits.One

        If Not mb.Connected Then
            mb.Connect()
        End If
        If mb.Connected Then
            CoilsIn = mb.ReadCoils(1, CoilsCount)
            InputStatusIn = mb.ReadDiscreteInputs(1, InputStatusCount)
            HoldingRegistersIn = mb.ReadHoldingRegisters(1, HoldingRegistersCount)
            InputRegistersIn = mb.ReadInputRegisters(1, InputRegistersCount)

            'Dim TempFloat = EasyModbus.ModbusClient.ConvertRegistersToDouble(TempInt32)
            'Label1.Text = TempFloat.ToString
            Label1.Text = InputRegistersIn(0)
            mb.Disconnect()
        End If

    End Sub

End Class