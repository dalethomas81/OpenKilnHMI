Public Class Main

    Dim CoilsCount As Int16 = 10
    Dim CoilsIn(CoilsCount) As Boolean
    Dim CoilsOut(CoilsCount) As Boolean

    Dim InputStatusCount As Int16 = 10
    Dim InputStatusIn(InputStatusCount) As Boolean
    Dim InputStatusOut(InputStatusCount) As Boolean

    Dim HoldingRegistersCount As Int16 = 50
    Dim HoldingRegistersIn(HoldingRegistersCount) As Integer
    Dim HoldingRegistersOut(HoldingRegistersCount) As Integer

    Dim InputRegistersCount As Int16 = 50
    Dim InputRegistersIn(InputRegistersCount) As Integer
    Dim InputRegistersOut(InputRegistersCount) As Integer

    Structure Kiln_Schedule_Segment
        Dim Name As String
        Dim Enabled As Boolean
        Dim HoldEnabled As Boolean
        Dim Setpoint As Double
        Dim RampRate As UInteger
        Dim SoakTime As UInteger
    End Structure

    Structure Kiln_Schedule
        Dim Name As String
        Dim NumberOfSegments As Integer
        Dim ChangeSelectedSegment As Integer
        Dim Segment As Kiln_Schedule_Segment
        Dim RemainingHours As Integer
        Dim RemainingMinutes As Integer
        Dim RemainingSeconds As Integer
    End Structure

    Structure Kiln_Command
        Dim SelectSchedule As Boolean
        Dim SelectedSchedule As Integer
        Dim Setpoint As Double
        Dim StartProfile As Boolean
        Dim StopProfile As Boolean
        Dim HoldRelease As Boolean
        Dim ThermalOverride As Boolean
        Dim WriteEeprom As Boolean
        'Dim SchSegEnabled As Boolean
        'Dim SchHolEnabled As Boolean
    End Structure

    Structure Kiln_Status
        Dim HoldReleaseRequest As Boolean
        Dim SafetyOk As Boolean
        Dim InProcess As Boolean
        Dim ThermalRunaway As Boolean
        Dim EepromWritten As Boolean
        Dim SegmentState As Integer
        Dim SegmentName As String
        Dim ScheduleName As String
    End Structure

    Structure Kiln_PID
        Dim Temperature As Double
        Dim Output As Integer
        Dim SSR As Boolean
        Dim P As Double
        Dim I As Double
        Dim D As Double
    End Structure

    Structure Kiln_TemperatureController
        Dim Upper As Kiln_PID
        Dim Lower As Kiln_PID
    End Structure

    Structure Kiln
        Dim Mode As Integer
        Dim Heartbeat As Integer
        Dim NumberOfSchedules As Integer
        Dim ChangeSelectedSchedule As Integer
        Dim Schedule As Kiln_Schedule
        Dim Command As Kiln_Command
        Dim Status As Kiln_Status
        Dim TemperatureController As Kiln_TemperatureController
    End Structure

    Dim Kiln_01 As New Kiln

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        GetModbusData()
        MapVariables()
        UpdateUI()
    End Sub

    Private Sub GetModbusData()
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
            mb.Disconnect()
        End If
    End Sub

    Private Sub MapVariables()
        Dim TempRegArray(1) As Integer
        ' coils
        Kiln_01.Command.SelectSchedule = CoilsIn(0)
        Kiln_01.Command.StartProfile = CoilsIn(1)
        Kiln_01.Command.StopProfile = CoilsIn(2)
        Kiln_01.Command.HoldRelease = CoilsIn(3)
        Kiln_01.Command.ThermalOverride = CoilsIn(4)
        Kiln_01.Command.WriteEeprom = CoilsIn(5)
        Kiln_01.Schedule.Segment.Enabled = CoilsIn(6)
        Kiln_01.Schedule.Segment.HoldEnabled = CoilsIn(7)
        ' input status
        Kiln_01.TemperatureController.Upper.SSR = InputStatusIn(0)
        Kiln_01.TemperatureController.Lower.SSR = InputStatusIn(1)
        Kiln_01.Status.HoldReleaseRequest = InputStatusIn(3)
        Kiln_01.Status.SafetyOk = InputStatusIn(4)
        Kiln_01.Status.InProcess = InputStatusIn(5)
        Kiln_01.Status.ThermalRunaway = InputStatusIn(6)
        Kiln_01.Status.EepromWritten = InputStatusIn(7)
        ' holding registers
        Kiln_01.Mode = HoldingRegistersIn(0)
        Kiln_01.Command.SelectedSchedule = HoldingRegistersIn(1)
        TempRegArray(0) = HoldingRegistersIn(2)
        TempRegArray(1) = HoldingRegistersIn(3)
        Kiln_01.Command.Setpoint = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(4)
        TempRegArray(1) = HoldingRegistersIn(5)
        Kiln_01.TemperatureController.Upper.P = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(6)
        TempRegArray(1) = HoldingRegistersIn(7)
        Kiln_01.TemperatureController.Upper.I = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(8)
        TempRegArray(1) = HoldingRegistersIn(9)
        Kiln_01.TemperatureController.Upper.D = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(10)
        TempRegArray(1) = HoldingRegistersIn(11)
        Kiln_01.TemperatureController.Lower.P = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(12)
        TempRegArray(1) = HoldingRegistersIn(13)
        Kiln_01.TemperatureController.Lower.I = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(14)
        TempRegArray(1) = HoldingRegistersIn(15)
        Kiln_01.TemperatureController.Lower.D = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        Kiln_01.Schedule.Name = EasyModbus.ModbusClient.ConvertRegistersToString(HoldingRegistersIn, 16, 16)
        Kiln_01.Schedule.Name = EasyModbus.ModbusClient.ConvertRegistersToString(HoldingRegistersIn, 24, 16)
        TempRegArray(0) = HoldingRegistersIn(32)
        TempRegArray(1) = HoldingRegistersIn(33)
        Kiln_01.Schedule.Segment.Setpoint = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        Kiln_01.Schedule.Segment.RampRate = HoldingRegistersIn(34)
        Kiln_01.Schedule.Segment.SoakTime = HoldingRegistersIn(35)
        Kiln_01.Schedule.ChangeSelectedSegment = HoldingRegistersIn(36)
        Kiln_01.ChangeSelectedSchedule = HoldingRegistersIn(37)
        ' input registers
        Kiln_01.Heartbeat = InputRegistersIn(0)
        Kiln_01.Schedule.RemainingHours = InputRegistersIn(1)
        Kiln_01.Schedule.RemainingMinutes = InputRegistersIn(2)
        Kiln_01.Schedule.RemainingSeconds = InputRegistersIn(3)
        TempRegArray(0) = InputRegistersIn(4)
        TempRegArray(1) = InputRegistersIn(5)
        Kiln_01.TemperatureController.Upper.Temperature = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(6)
        TempRegArray(1) = InputRegistersIn(7)
        Kiln_01.TemperatureController.Lower.Temperature = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(8)
        TempRegArray(1) = InputRegistersIn(9)
        Kiln_01.TemperatureController.Upper.Output = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(10)
        TempRegArray(1) = InputRegistersIn(11)
        Kiln_01.TemperatureController.Lower.Output = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        Kiln_01.NumberOfSchedules = InputRegistersIn(12)
        Kiln_01.Schedule.NumberOfSegments = InputRegistersIn(13)
        Kiln_01.Status.SegmentState = InputRegistersIn(14)
        Kiln_01.Status.SegmentName = EasyModbus.ModbusClient.ConvertRegistersToString(InputRegistersIn, 15, 16)
        Kiln_01.Status.ScheduleName = EasyModbus.ModbusClient.ConvertRegistersToString(InputRegistersIn, 23, 16)
    End Sub

    Private Sub UpdateUI()
        Label1.Text = Kiln_01.Heartbeat
    End Sub

End Class