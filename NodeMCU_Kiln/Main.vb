Public Class Main
    ' coils (RW) 
    Private Const MB_CMD_SELECT_SCHEDULE As Integer = 1
    Private Const MB_CMD_START_PROFILE As Integer = 2
    Private Const MB_CMD_STOP_PROFILE As Integer = 3
    Private Const MB_CMD_HOLD_RELEASE As Integer = 4
    Private Const MB_CMD_THERM_OVERRIDE As Integer = 5
    Private Const MB_CMD_WRITE_EEPROM As Integer = 6
    Private Const MB_SCH_SEG_ENABLED As Integer = 7
    Private Const MB_SCH_SEG_HOLD_EN As Integer = 8
    ' input status (R)
    Private Const MB_STS_SSR_01 As Integer = 1
    Private Const MB_STS_SSR_02 As Integer = 2
    Private Const MB_STS_RELEASE_REQ As Integer = 3
    Private Const MB_STS_SAFETY_OK As Integer = 4
    Private Const MB_STS_IN_PROCESS As Integer = 5
    Private Const MB_STS_THERMAL_RUNAWAY As Integer = 6
    Private Const MB_STS_EEPROM_WRITTEN As Integer = 7
    ' holding registers (RW) 16 bit
    Private Const MB_MODE As Integer = 1
    Private Const MB_CMD_SELECTED_SCHEDULE As Integer = 2
    Private Const MB_CMD_SETPOINT As Integer = 3
    Private Const MB_PID_P_01 As Integer = 5
    Private Const MB_PID_I_01 As Integer = 7
    Private Const MB_PID_D_01 As Integer = 9
    Private Const MB_PID_P_02 As Integer = 11
    Private Const MB_PID_I_02 As Integer = 13
    Private Const MB_PID_D_02 As Integer = 15
    Private Const MB_SCH_NAME As Integer = 17
    Private Const MB_SCH_SEG_NAME As Integer = 25
    Private Const MB_SCH_SEG_SETPOINT As Integer = 33
    Private Const MB_SCH_SEG_RAMP_RATE As Integer = 35
    Private Const MB_SCH_SEG_SOAK_TIME As Integer = 36
    Private Const MB_SCH_SEG_SELECTED As Integer = 37
    Private Const MB_SCH_SELECTED As Integer = 38
    ' input registers (R) 16 bit
    Private Const MB_HEARTBEAT As Integer = 1
    Private Const MB_STS_REMAINING_TIME_H As Integer = 2
    Private Const MB_STS_REMAINING_TIME_M As Integer = 3
    Private Const MB_STS_REMAINING_TIME_S As Integer = 4
    Private Const MB_STS_TEMPERATURE_01 As Integer = 5
    Private Const MB_STS_TEMPERATURE_02 As Integer = 7
    Private Const MB_STS_PID_01_OUTPUT As Integer = 9
    Private Const MB_STS_PID_02_OUTPUT As Integer = 11
    Private Const MB_NUMBER_OF_SCHEDULES As Integer = 13
    Private Const MB_NUMBER_OF_SEGMENTS As Integer = 14
    Private Const MB_STS_SEGMENT_STATE As Integer = 15
    Private Const MB_STS_SEGMENT_NAME As Integer = 16
    Private Const MB_STS_SCHEDULE_NAME As Integer = 24

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
        Dim Heartbeat As Integer
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

    Private Sub SendModbusCoil(ByVal Address As Integer, ByVal Value As Boolean)
        Dim mb As New EasyModbus.ModbusClient("COM26")
        mb.Baudrate = 115200
        mb.Parity = IO.Ports.Parity.None
        mb.UnitIdentifier = 1
        mb.StopBits = IO.Ports.StopBits.One

        If Not mb.Connected Then
            mb.Connect()
        End If
        If mb.Connected Then
            mb.WriteSingleCoil(Address, Value)
            mb.Disconnect()
        End If
    End Sub

    Private Sub SendModbusHoldingRegister(ByVal Address As Integer, ByVal Value As Integer)
        Dim mb As New EasyModbus.ModbusClient("COM26")
        mb.Baudrate = 115200
        mb.Parity = IO.Ports.Parity.None
        mb.UnitIdentifier = 1
        mb.StopBits = IO.Ports.StopBits.One

        If Not mb.Connected Then
            mb.Connect()
        End If
        If mb.Connected Then
            mb.WriteSingleRegister(Address, Value)
            mb.Disconnect()
        End If
    End Sub

    Private Sub MapVariables()
        Dim TempRegArray(1) As Integer
        ' coils
        Kiln_01.Command.SelectSchedule = CoilsIn(MB_CMD_SELECT_SCHEDULE - 1)
        Kiln_01.Command.StartProfile = CoilsIn(MB_CMD_START_PROFILE - 1)
        Kiln_01.Command.StopProfile = CoilsIn(MB_CMD_STOP_PROFILE - 1)
        Kiln_01.Command.HoldRelease = CoilsIn(MB_CMD_HOLD_RELEASE - 1)
        Kiln_01.Command.ThermalOverride = CoilsIn(MB_CMD_THERM_OVERRIDE - 1)
        Kiln_01.Command.WriteEeprom = CoilsIn(MB_CMD_WRITE_EEPROM - 1)
        Kiln_01.Schedule.Segment.Enabled = CoilsIn(MB_SCH_SEG_ENABLED - 1)
        Kiln_01.Schedule.Segment.HoldEnabled = CoilsIn(MB_SCH_SEG_HOLD_EN - 1)
        ' input status
        Kiln_01.TemperatureController.Upper.SSR = InputStatusIn(MB_STS_SSR_01 - 1)
        Kiln_01.TemperatureController.Lower.SSR = InputStatusIn(MB_STS_SSR_02 - 1)
        Kiln_01.Status.HoldReleaseRequest = InputStatusIn(MB_STS_RELEASE_REQ - 1)
        Kiln_01.Status.SafetyOk = InputStatusIn(MB_STS_SAFETY_OK - 1)
        Kiln_01.Status.InProcess = InputStatusIn(MB_STS_IN_PROCESS - 1)
        Kiln_01.Status.ThermalRunaway = InputStatusIn(MB_STS_THERMAL_RUNAWAY - 1)
        Kiln_01.Status.EepromWritten = InputStatusIn(MB_STS_EEPROM_WRITTEN - 1)
        ' holding registers
        Kiln_01.Mode = HoldingRegistersIn(MB_MODE - 1)
        Kiln_01.Command.SelectedSchedule = HoldingRegistersIn(MB_CMD_SELECTED_SCHEDULE - 1)
        TempRegArray(0) = HoldingRegistersIn(MB_CMD_SETPOINT - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_CMD_SETPOINT + 1 - 1)
        Kiln_01.Command.Setpoint = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_PID_P_01 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_PID_P_01 + 1 - 1)
        Kiln_01.TemperatureController.Upper.P = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_PID_I_01 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_PID_I_01 + 1 - 1)
        Kiln_01.TemperatureController.Upper.I = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_PID_D_01 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_PID_D_01 + 1 - 1)
        Kiln_01.TemperatureController.Upper.D = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_PID_P_02 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_PID_P_02 + 1 - 1)
        Kiln_01.TemperatureController.Lower.P = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_PID_I_02 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_PID_I_02 + 1 - 1)
        Kiln_01.TemperatureController.Lower.I = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_PID_D_02 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_PID_D_02 + 1 - 1)
        Kiln_01.TemperatureController.Lower.D = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        Kiln_01.Schedule.Name = EasyModbus.ModbusClient.ConvertRegistersToString(HoldingRegistersIn, MB_SCH_NAME - 1, 16)
        Kiln_01.Schedule.Segment.Name = EasyModbus.ModbusClient.ConvertRegistersToString(HoldingRegistersIn, MB_SCH_SEG_NAME - 1, 16)
        TempRegArray(0) = HoldingRegistersIn(MB_SCH_SEG_SETPOINT - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_SCH_SEG_SETPOINT + 1 - 1)
        Kiln_01.Schedule.Segment.Setpoint = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        Kiln_01.Schedule.Segment.RampRate = HoldingRegistersIn(MB_SCH_SEG_RAMP_RATE - 1)
        Kiln_01.Schedule.Segment.SoakTime = HoldingRegistersIn(MB_SCH_SEG_SOAK_TIME - 1)
        Kiln_01.Schedule.ChangeSelectedSegment = HoldingRegistersIn(MB_SCH_SEG_SELECTED - 1)
        Kiln_01.ChangeSelectedSchedule = HoldingRegistersIn(MB_SCH_SELECTED - 1)
        ' input registers
        Kiln_01.Status.Heartbeat = InputRegistersIn(MB_HEARTBEAT - 1)
        Kiln_01.Schedule.RemainingHours = InputRegistersIn(MB_STS_REMAINING_TIME_H - 1)
        Kiln_01.Schedule.RemainingMinutes = InputRegistersIn(MB_STS_REMAINING_TIME_M - 1)
        Kiln_01.Schedule.RemainingSeconds = InputRegistersIn(MB_STS_REMAINING_TIME_S - 1)
        TempRegArray(0) = InputRegistersIn(MB_STS_TEMPERATURE_01 - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_TEMPERATURE_01 + 1 - 1)
        Kiln_01.TemperatureController.Upper.Temperature = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_TEMPERATURE_02 - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_TEMPERATURE_02 + 1 - 1)
        Kiln_01.TemperatureController.Lower.Temperature = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_PID_01_OUTPUT - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_PID_01_OUTPUT + 1 - 1)
        Kiln_01.TemperatureController.Upper.Output = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_PID_02_OUTPUT - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_PID_02_OUTPUT + 1 - 1)
        Kiln_01.TemperatureController.Lower.Output = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        Kiln_01.NumberOfSchedules = InputRegistersIn(MB_NUMBER_OF_SCHEDULES - 1)
        Kiln_01.Schedule.NumberOfSegments = InputRegistersIn(MB_NUMBER_OF_SEGMENTS - 1)
        Kiln_01.Status.SegmentState = InputRegistersIn(MB_STS_SEGMENT_STATE - 1)
        Kiln_01.Status.SegmentName = EasyModbus.ModbusClient.ConvertRegistersToString(InputRegistersIn, MB_STS_SEGMENT_NAME - 1, 16)
        Kiln_01.Status.ScheduleName = EasyModbus.ModbusClient.ConvertRegistersToString(InputRegistersIn, MB_STS_SCHEDULE_NAME - 1, 16)
    End Sub

    Private Sub UpdateUI()
        ToolStripStatusLabel1.Text = "Heartbeat: " & Kiln_01.Status.Heartbeat
        If Kiln_01.Status.SafetyOk Then
            ToolStripStatusLabel2.Text = "Safety Circuit Ok"
            ToolStripStatusLabel2.BackColor = Color.YellowGreen
        Else
            ToolStripStatusLabel2.Text = "Safety Circuit Not Ok"
            ToolStripStatusLabel2.BackColor = Color.Tomato
        End If
        Select Case Kiln_01.Mode
            Case 1  ' auto
                ToolStripStatusLabel3.Text = "Mode: Automatic"
                ToolStripStatusLabel3.BackColor = Color.YellowGreen
                Button3.Text = "Auto"
                Button3.BackColor = Color.YellowGreen
            Case 2  ' manual
                ToolStripStatusLabel3.Text = "Mode: Manual"
                ToolStripStatusLabel3.BackColor = Color.SkyBlue
                Button3.Text = "Manual"
                Button3.BackColor = Color.SkyBlue
            Case 3  ' simulation
                ToolStripStatusLabel3.Text = "Mode: Simulation"
                ToolStripStatusLabel3.BackColor = Color.DarkOrange
                Button3.Text = "Simulation"
                Button3.BackColor = Color.DarkOrange
            Case Else
                ToolStripStatusLabel3.Text = "Mode: unknown"
                ToolStripStatusLabel3.BackColor = Color.Tomato
                Button3.Text = "unknown"
                Button3.BackColor = Color.Tomato
        End Select
        Label1.Text = "Segment: " & Kiln_01.Schedule.Segment.Name
        Label2.Text = "Schedule: " & Kiln_01.Schedule.Name

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SendModbusCoil(MB_CMD_START_PROFILE, True)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SendModbusCoil(MB_CMD_STOP_PROFILE, True)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim ModeOut As Integer = (Kiln_01.Mode + 1) Mod 4
        If ModeOut <= 0 Then ' 0 is not a valid mode - filter it and anything below
            ModeOut = 1
        End If
        SendModbusHoldingRegister(MB_MODE, ModeOut)
    End Sub

End Class