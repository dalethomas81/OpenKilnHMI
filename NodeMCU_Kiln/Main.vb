﻿Public Class MAIN
    Private SerialPort = "COM18"
    Private IPAddress = "10.0.1.164"
    'Private COM_MODE = "RTU"
    Private COM_MODE = "TCP"
    Public Const MAX_STRING_LENGTH = 15 ' 16 but leave room for '\0'
    ' coils (RW) 
    Public Const MB_CMD_SELECT_SCHEDULE As Integer = 1
    Public Const MB_CMD_START_PROFILE As Integer = 2
    Public Const MB_CMD_STOP_PROFILE As Integer = 3
    Public Const MB_CMD_HOLD_RELEASE As Integer = 4
    Public Const MB_CMD_THERM_OVERRIDE As Integer = 5
    Public Const MB_CMD_WRITE_EEPROM As Integer = 6
    Public Const MB_SCH_SEG_ENABLED As Integer = 7
    Public Const MB_SCH_SEG_HOLD_EN As Integer = 8
    Public Const MB_CMD_CAL_CH0_LOW As Integer = 9
    Public Const MB_CMD_CAL_CH1_LOW As Integer = 10
    Public Const MB_CMD_CAL_CH0_HIGH As Integer = 11
    Public Const MB_CMD_CAL_CH1_HIGH As Integer = 12
    ' input status (R)
    Public Const MB_STS_SSR_01 As Integer = 1
    Public Const MB_STS_SSR_02 As Integer = 2
    Public Const MB_STS_RELEASE_REQ As Integer = 3
    Public Const MB_STS_SAFETY_OK As Integer = 4
    Public Const MB_STS_IN_PROCESS As Integer = 5
    Public Const MB_STS_THERMAL_RUNAWAY As Integer = 6
    Public Const MB_STS_EEPROM_WRITTEN As Integer = 7
    ' holding registers (RW) 16 bit
    Public Const MB_MODE As Integer = 1
    Public Const MB_CMD_SELECTED_SCHEDULE As Integer = 2
    Public Const MB_CMD_SETPOINT As Integer = 3
    Public Const MB_PID_P_01 As Integer = 5
    Public Const MB_PID_I_01 As Integer = 7
    Public Const MB_PID_D_01 As Integer = 9
    Public Const MB_PID_P_02 As Integer = 11
    Public Const MB_PID_I_02 As Integer = 13
    Public Const MB_PID_D_02 As Integer = 15
    Public Const MB_SCH_NAME As Integer = 17
    Public Const MB_SCH_SEG_NAME As Integer = 25
    Public Const MB_SCH_SEG_SETPOINT As Integer = 33
    Public Const MB_SCH_SEG_RAMP_RATE As Integer = 35
    Public Const MB_SCH_SEG_SOAK_TIME As Integer = 36
    Public Const MB_SCH_SEG_SELECTED As Integer = 37
    Public Const MB_SCH_SELECTED As Integer = 38
    Public Const MB_CAL_TEMP_ACT_CH0 As Integer = 39
    Public Const MB_CAL_TEMP_ACT_CH1 As Integer = 41
    ' input registers (R) 16 bit
    Public Const MB_HEARTBEAT As Integer = 1
    Public Const MB_STS_REMAINING_TIME_H As Integer = 2
    Public Const MB_STS_REMAINING_TIME_M As Integer = 3
    Public Const MB_STS_REMAINING_TIME_S As Integer = 4
    Public Const MB_STS_TEMPERATURE_01 As Integer = 5
    Public Const MB_STS_TEMPERATURE_02 As Integer = 7
    Public Const MB_STS_PID_01_OUTPUT As Integer = 9
    Public Const MB_STS_PID_02_OUTPUT As Integer = 11
    Public Const MB_NUMBER_OF_SCHEDULES As Integer = 13
    Public Const MB_NUMBER_OF_SEGMENTS As Integer = 14
    Public Const MB_STS_SEGMENT_STATE As Integer = 15
    Public Const MB_STS_SEGMENT_NAME As Integer = 16
    Public Const MB_STS_SCHEDULE_NAME As Integer = 24
    Public Const MB_STS_TEMP_01_RAW As Integer = 32
    Public Const MB_STS_TEMP_02_RAW As Integer = 34
    Public Const MB_STS_RUNAWAY_TEMP_T As Integer = 36
    Public Const MB_STS_RUNAWAY_RATE_T As Integer = 38
    Public Const MB_STS_MEAS_RATE_CH0 As Integer = 40
    Public Const MB_STS_MEAS_RATE_CH1 As Integer = 42

    Dim CoilsCount As Int16 = 15
    Dim CoilsIn(CoilsCount) As Boolean
    'Dim CoilsOut(CoilsCount) As Boolean

    Dim InputStatusCount As Int16 = 15
    Dim InputStatusIn(InputStatusCount) As Boolean
    'Dim InputStatusOut(InputStatusCount) As Boolean

    Dim HoldingRegistersCount As Int16 = 42
    Dim HoldingRegistersIn(HoldingRegistersCount) As Integer
    'Dim HoldingRegistersOut(HoldingRegistersCount) As Integer

    Dim InputRegistersCount As Int16 = 44
    Dim InputRegistersIn(InputRegistersCount) As Integer
    'Dim InputRegistersOut(InputRegistersCount) As Integer

    Dim HeartbeatLast As Integer = 0
    Dim HeartbeatOk As Boolean = False

    Structure Kiln_Schedule_Segment
        Dim Name As String
        Dim Enabled As Boolean
        Dim HoldEnabled As Boolean
        Dim Setpoint As Double
        Dim RampRate As Integer
        Dim SoakTime As Integer
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
        Dim RunawayTemperatureTimer As Integer
        Dim RunawayRateTimer As Integer
    End Structure

    Structure Kiln_PID
        Dim Temperature As Double
        Dim Temperature_raw As Double
        Dim Temperature_actual As Double
        Dim MeasuredRate As Double
        Dim Cal_Map_low As Boolean
        Dim Cal_Map_high As Boolean
        Dim Output As Single
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

    Public Kiln_01 As New Kiln

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        GetModbusData()
        MapVariables()
        UpdateUI()
    End Sub

    Private Sub GetModbusData()
        Try
            Dim mb As New EasyModbus.ModbusClient
            'Dim mb As New EasyModbus.ModbusClient(IPAddress, 502)
            'Dim mb As New EasyModbus.ModbusClient(SerialPort)
            'mb.Baudrate = 115200
            'mb.Parity = IO.Ports.Parity.None
            'mb.UnitIdentifier = 1
            'mb.StopBits = IO.Ports.StopBits.One

            Select Case COM_MODE
                Case "RTU"
                    mb = New EasyModbus.ModbusClient(SerialPort)
                    mb.Baudrate = 115200
                    mb.Parity = IO.Ports.Parity.None
                    mb.UnitIdentifier = 1
                    mb.StopBits = IO.Ports.StopBits.One
                Case "TCP"
                    mb = New EasyModbus.ModbusClient(IPAddress, 502)
                    mb.UnitIdentifier = 1
            End Select

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
        Catch ex As Exception

        End Try
    End Sub

    Public Sub SendModbusCoil(ByVal Address As Integer, ByVal Value As Boolean)
        Try
            Dim mb As New EasyModbus.ModbusClient
            'Dim mb As New EasyModbus.ModbusClient(IPAddress, 502)
            'Dim mb As New EasyModbus.ModbusClient(SerialPort)
            'mb.Baudrate = 115200
            'mb.Parity = IO.Ports.Parity.None
            'mb.UnitIdentifier = 1
            'mb.StopBits = IO.Ports.StopBits.One

            Select Case COM_MODE
                Case "RTU"
                    mb = New EasyModbus.ModbusClient(SerialPort)
                    mb.Baudrate = 115200
                    mb.Parity = IO.Ports.Parity.None
                    mb.UnitIdentifier = 1
                    mb.StopBits = IO.Ports.StopBits.One
                Case "TCP"
                    mb = New EasyModbus.ModbusClient(IPAddress, 502)
                    mb.UnitIdentifier = 1
            End Select

            If Not mb.Connected Then
                mb.Connect()
            End If
            If mb.Connected Then
                mb.WriteSingleCoil(Address, Value)
                mb.Disconnect()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub SendModbusHoldingRegister(ByVal Address As Integer, ByVal Value As Integer)
        Try
            Dim mb As New EasyModbus.ModbusClient
            'Dim mb As New EasyModbus.ModbusClient(IPAddress, 502)
            'Dim mb As New EasyModbus.ModbusClient(SerialPort)
            'mb.Baudrate = 115200
            'mb.Parity = IO.Ports.Parity.None
            'mb.UnitIdentifier = 1
            'mb.StopBits = IO.Ports.StopBits.One

            Select Case COM_MODE
                Case "RTU"
                    mb = New EasyModbus.ModbusClient(SerialPort)
                    mb.Baudrate = 115200
                    mb.Parity = IO.Ports.Parity.None
                    mb.UnitIdentifier = 1
                    mb.StopBits = IO.Ports.StopBits.One
                Case "TCP"
                    mb = New EasyModbus.ModbusClient(IPAddress, 502)
                    mb.UnitIdentifier = 1
            End Select

            If Not mb.Connected Then
                mb.Connect()
            End If
            If mb.Connected Then
                mb.WriteSingleRegister(Address, Value)
                mb.Disconnect()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub SendModbusMultipleHoldingRegisters(ByVal Address As Integer, ByVal Values() As Integer)
        Try
            Dim mb As New EasyModbus.ModbusClient
            'Dim mb As New EasyModbus.ModbusClient(IPAddress, 502)
            'Dim mb As New EasyModbus.ModbusClient(SerialPort)
            'mb.Baudrate = 115200
            'mb.Parity = IO.Ports.Parity.None
            'mb.UnitIdentifier = 1
            'mb.StopBits = IO.Ports.StopBits.One

            Select Case COM_MODE
                Case "RTU"
                    mb = New EasyModbus.ModbusClient(SerialPort)
                    mb.Baudrate = 115200
                    mb.Parity = IO.Ports.Parity.None
                    mb.UnitIdentifier = 1
                    mb.StopBits = IO.Ports.StopBits.One
                Case "TCP"
                    mb = New EasyModbus.ModbusClient(IPAddress, 502)
                    mb.UnitIdentifier = 1
            End Select

            If Not mb.Connected Then
                mb.Connect()
            End If
            If mb.Connected Then
                mb.WriteMultipleRegisters(Address, Values)
                mb.Disconnect()
            End If
        Catch ex As Exception

        End Try
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
        Kiln_01.TemperatureController.Upper.Cal_Map_low = CoilsIn(MB_CMD_CAL_CH0_LOW - 1)
        Kiln_01.TemperatureController.Lower.Cal_Map_low = CoilsIn(MB_CMD_CAL_CH1_LOW - 1)
        Kiln_01.TemperatureController.Upper.Cal_Map_high = CoilsIn(MB_CMD_CAL_CH0_HIGH - 1)
        Kiln_01.TemperatureController.Lower.Cal_Map_high = CoilsIn(MB_CMD_CAL_CH1_HIGH - 1)
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
        TempRegArray(0) = HoldingRegistersIn(MB_CAL_TEMP_ACT_CH0 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_CAL_TEMP_ACT_CH0 + 1 - 1)
        Kiln_01.TemperatureController.Upper.Temperature_actual = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = HoldingRegistersIn(MB_CAL_TEMP_ACT_CH1 - 1)
        TempRegArray(1) = HoldingRegistersIn(MB_CAL_TEMP_ACT_CH1 + 1 - 1)
        Kiln_01.TemperatureController.Lower.Temperature_actual = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
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
        TempRegArray(0) = InputRegistersIn(MB_STS_TEMP_01_RAW - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_TEMP_01_RAW + 1 - 1)
        Kiln_01.TemperatureController.Upper.Temperature_raw = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_TEMP_02_RAW - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_TEMP_02_RAW + 1 - 1)
        Kiln_01.TemperatureController.Lower.Temperature_raw = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_RUNAWAY_TEMP_T - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_RUNAWAY_TEMP_T + 1 - 1)
        Kiln_01.Status.RunawayTemperatureTimer = EasyModbus.ModbusClient.ConvertRegistersToInt(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_RUNAWAY_RATE_T - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_RUNAWAY_RATE_T + 1 - 1)
        Kiln_01.Status.RunawayRateTimer = EasyModbus.ModbusClient.ConvertRegistersToInt(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_MEAS_RATE_CH0 - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_MEAS_RATE_CH0 + 1 - 1)
        Kiln_01.TemperatureController.Upper.MeasuredRate = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
        TempRegArray(0) = InputRegistersIn(MB_STS_MEAS_RATE_CH1 - 1)
        TempRegArray(1) = InputRegistersIn(MB_STS_MEAS_RATE_CH1 + 1 - 1)
        Kiln_01.TemperatureController.Lower.MeasuredRate = EasyModbus.ModbusClient.ConvertRegistersToFloat(TempRegArray)
    End Sub

    Private Sub UpdateUI()

        If Kiln_01.Status.Heartbeat = HeartbeatLast Then
            Timer2.Enabled = True
        Else
            Timer2.Enabled = False
            HeartbeatOk = True
        End If
        HeartbeatLast = Kiln_01.Status.Heartbeat

        If HeartbeatOk Then
            ToolStripStatusLabel1.BackColor = Color.YellowGreen
        Else
            ToolStripStatusLabel1.BackColor = Color.Tomato
        End If
        ToolStripStatusLabel1.Text = "HEARTBEAT: " & Kiln_01.Status.Heartbeat
        If Kiln_01.Status.SafetyOk Then
            ToolStripStatusLabel2.Text = "SAFETY CIRCUIT OK"
            ToolStripStatusLabel2.BackColor = Color.YellowGreen
        Else
            ToolStripStatusLabel2.Text = "SAFETY CIRCUIT NOT OK"
            ToolStripStatusLabel2.BackColor = Color.Tomato
        End If
        If Kiln_01.Status.ThermalRunaway Then
            ToolStripStatusLabel4.Text = "THERMAL RUNAWAY DETECTED"
            ToolStripStatusLabel4.BackColor = Color.Tomato
        Else
            If Kiln_01.Command.ThermalOverride Then
                ToolStripStatusLabel4.Text = "THERMAL MONITORING DISABLED"
                ToolStripStatusLabel4.BackColor = Color.DarkOrange
            Else
                ToolStripStatusLabel4.Text = "THERMAL MONITORING OK"
                ToolStripStatusLabel4.BackColor = Color.YellowGreen
            End If
        End If
        Select Case Kiln_01.Mode
            Case 1  ' auto
                ToolStripStatusLabel3.Text = "MODE: AUTOMATIC"
                ToolStripStatusLabel3.BackColor = Color.YellowGreen
                Button3.Text = "AUTOMATIC"
                Button3.BackColor = Color.YellowGreen
            Case 2  ' manual
                ToolStripStatusLabel3.Text = "MODE: MANUAL"
                ToolStripStatusLabel3.BackColor = Color.SkyBlue
                Button3.Text = "MANUAL"
                Button3.BackColor = Color.SkyBlue
            Case 3  ' simulation
                ToolStripStatusLabel3.Text = "MODE: SIMULATION"
                ToolStripStatusLabel3.BackColor = Color.DarkOrange
                Button3.Text = "SIMULATION"
                Button3.BackColor = Color.DarkOrange
            Case Else
                ToolStripStatusLabel3.Text = "MODE: UNKNOWN"
                ToolStripStatusLabel3.BackColor = Color.Tomato
                Button3.Text = "UNKNOWN"
                Button3.BackColor = Color.Tomato
        End Select
        Label12.Text = Kiln_01.Command.SelectedSchedule & " - " & Kiln_01.Status.ScheduleName
        Label14.Text = Kiln_01.Status.SegmentName
        Label9.Text = Kiln_01.Schedule.RemainingHours.ToString("D2") & ":" &
                    Kiln_01.Schedule.RemainingMinutes.ToString("D2") & ":" &
                    Kiln_01.Schedule.RemainingSeconds.ToString("D2")
        Label10.Text = Math.Round(Kiln_01.Command.Setpoint, 2) & " F"
        Label13.Text = Math.Round(Kiln_01.TemperatureController.Upper.Temperature, 2) & " F"
        Label11.Text = Math.Round(Kiln_01.TemperatureController.Lower.Temperature, 2) & " F"
        Select Case Kiln_01.Status.SegmentState
            Case 0
                Label8.Text = "IDLE"
            Case 1
                Label8.Text = "RAMP"
            Case 2
                Label8.Text = "SOAK"
            Case 3
                Label8.Text = "HOLD"
            Case 4
                Label8.Text = "INIT"
            Case 5
                Label8.Text = "START"
            Case Else

        End Select
        If Kiln_01.Status.HoldReleaseRequest Then
            Button6.Visible = True
        Else
            Button6.Visible = False
        End If

        SCHEDULE.Label4.Text = Kiln_01.ChangeSelectedSchedule & " - " & Kiln_01.Schedule.Name
        SCHEDULE.Label5.Text = Kiln_01.Schedule.ChangeSelectedSegment & " - " & Kiln_01.Schedule.Segment.Name

        SCHEDULE.CheckBox1.Checked = Kiln_01.Schedule.Segment.Enabled

        SCHEDULE.CheckBox2.Checked = Kiln_01.Schedule.Segment.HoldEnabled

        If Kiln_01.Status.EepromWritten Then
            SCHEDULE.Label3.Visible = True
            PID_SETTINGS.Label4.Visible = True
            TUNING.Label29.Visible = True
        Else
            SCHEDULE.Label3.Visible = False
            PID_SETTINGS.Label4.Visible = False
            TUNING.Label29.Visible = False
        End If

        SCHEDULE.Label9.Text = Kiln_01.Schedule.Segment.Setpoint
        SCHEDULE.Label10.Text = Kiln_01.Schedule.Segment.RampRate
        SCHEDULE.Label11.Text = Kiln_01.Schedule.Segment.SoakTime

        TUNING.CheckBox1.Checked = Kiln_01.Command.ThermalOverride

        TUNING.Label3.Text = Math.Round(Kiln_01.TemperatureController.Upper.Temperature_raw, 2) & " F"
        TUNING.Label4.Text = Math.Round(Kiln_01.TemperatureController.Lower.Temperature_raw, 2) & " F"
        TUNING.Label7.Text = Math.Round(Kiln_01.TemperatureController.Upper.Output, 2)
        TUNING.Label5.Text = Math.Round(Kiln_01.TemperatureController.Lower.Output, 2)

        TUNING.Label9.Text = Math.Round(Kiln_01.TemperatureController.Upper.P, 4)
        TUNING.Label11.Text = Math.Round(Kiln_01.TemperatureController.Upper.I, 4)
        TUNING.Label13.Text = Math.Round(Kiln_01.TemperatureController.Upper.D, 4)

        TUNING.Label19.Text = Math.Round(Kiln_01.TemperatureController.Lower.P, 4)
        TUNING.Label17.Text = Math.Round(Kiln_01.TemperatureController.Lower.I, 4)
        TUNING.Label15.Text = Math.Round(Kiln_01.TemperatureController.Lower.D, 4)

        'SETTINGS.NumericUpDown1.Value = Math.Round(Kiln_01.TemperatureController.Upper.Temperature_actual, 2)
        'SETTINGS.NumericUpDown2.Value = Math.Round(Kiln_01.TemperatureController.Lower.Temperature_actual, 2)

        TUNING.Label23.Text = Math.Round(Kiln_01.TemperatureController.Upper.Temperature, 2) & " F"
        TUNING.Label28.Text = Math.Round(Kiln_01.TemperatureController.Lower.Temperature, 2) & " F"

        TUNING.Label32.Text = Math.Round(Kiln_01.Status.RunawayTemperatureTimer / 1000, 2) & " s"
        TUNING.Label30.Text = Math.Round(Kiln_01.Status.RunawayRateTimer / 1000, 2) & " s"

        TUNING.Label36.Text = Math.Round(Kiln_01.TemperatureController.Upper.MeasuredRate, 2) & " F/hr"
        TUNING.Label34.Text = Math.Round(Kiln_01.TemperatureController.Lower.MeasuredRate, 2) & " F/hr"

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SendModbusCoil(MB_CMD_START_PROFILE, True)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SendModbusCoil(MB_CMD_STOP_PROFILE, True)
    End Sub

    ' change mode
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim ModeOut As Integer = (Kiln_01.Mode + 1) Mod 4
        If ModeOut <= 0 Then ' 0 is not a valid mode - filter it and anything below
            ModeOut = 1
        End If
        SendModbusHoldingRegister(MB_MODE, ModeOut)
    End Sub

    ' increment selected schedule
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim ScheduleOut As Integer = (Kiln_01.Command.SelectedSchedule + 1) Mod Kiln_01.NumberOfSchedules
        If ScheduleOut <= 0 Then
            ScheduleOut = 0
        ElseIf ScheduleOut >= Kiln_01.NumberOfSchedules Then
            ScheduleOut = Kiln_01.NumberOfSchedules - 1
        End If
        SendModbusHoldingRegister(MB_CMD_SELECTED_SCHEDULE, ScheduleOut)
    End Sub

    ' decrement selected schedule
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim ScheduleOut As Integer = (Kiln_01.Command.SelectedSchedule - 1) Mod Kiln_01.NumberOfSchedules
        If ScheduleOut < 0 Then
            ScheduleOut = Kiln_01.NumberOfSchedules - 1
        ElseIf ScheduleOut >= Kiln_01.NumberOfSchedules Then
            ScheduleOut = Kiln_01.NumberOfSchedules - 1
        End If
        SendModbusHoldingRegister(MB_CMD_SELECTED_SCHEDULE, ScheduleOut)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        SendModbusCoil(MB_CMD_HOLD_RELEASE, True)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        SCHEDULE.Show()
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        HeartbeatOk = False
    End Sub

    Private Sub MAIN_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' prevent resize
        Me.MinimumSize = Me.Size
        Me.MaximumSize = Me.Size
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        'SETTINGS.CheckBox1.Checked = Kiln_01.Command.ThermalOverride
        TUNING.NumericUpDown1.Value = Kiln_01.TemperatureController.Upper.Temperature_actual
        TUNING.NumericUpDown2.Value = Kiln_01.TemperatureController.Lower.Temperature_actual
        TUNING.Show()
    End Sub

    Private Sub Label10_Click(sender As Object, e As EventArgs) Handles Label10.Click
        SETPOINT.NumericUpDown1.Value = Kiln_01.Command.Setpoint
        SETPOINT.Show()
    End Sub
End Class