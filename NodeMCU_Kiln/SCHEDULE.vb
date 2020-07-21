Public Class SCHEDULE
    ' increment selected schedule
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim ScheduleOut As Integer = (Main.Kiln_01.ChangeSelectedSchedule + 1) Mod Main.Kiln_01.NumberOfSchedules
        If ScheduleOut <= 0 Then
            ScheduleOut = 0
        ElseIf ScheduleOut >= Main.Kiln_01.NumberOfSchedules Then
            ScheduleOut = Main.Kiln_01.NumberOfSchedules - 1
        End If
        Main.SendModbusHoldingRegister(Main.MB_SCH_SELECTED, ScheduleOut)
    End Sub

    ' decrement selected 
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim ScheduleOut As Integer = (Main.Kiln_01.ChangeSelectedSchedule - 1) Mod Main.Kiln_01.NumberOfSchedules
        If ScheduleOut < 0 Then
            ScheduleOut = Main.Kiln_01.NumberOfSchedules - 1
        ElseIf ScheduleOut >= Main.Kiln_01.NumberOfSchedules Then
            ScheduleOut = Main.Kiln_01.NumberOfSchedules - 1
        End If
        Main.SendModbusHoldingRegister(Main.MB_SCH_SELECTED, ScheduleOut)

    End Sub

    ' increment selected segment
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim SegmentOut As Integer = (Main.Kiln_01.Schedule.ChangeSelectedSegment + 1) Mod Main.Kiln_01.Schedule.NumberOfSegments
        If SegmentOut <= 0 Then
            SegmentOut = 0
        ElseIf SegmentOut >= Main.Kiln_01.Schedule.NumberOfSegments Then
            SegmentOut = Main.Kiln_01.Schedule.NumberOfSegments - 1
        End If
        Main.SendModbusHoldingRegister(Main.MB_SCH_SEG_SELECTED, SegmentOut)
    End Sub

    ' decrement selected segment
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim SegmentOut As Integer = (Main.Kiln_01.Schedule.ChangeSelectedSegment - 1) Mod Main.Kiln_01.Schedule.NumberOfSegments
        If SegmentOut < 0 Then
            SegmentOut = Main.Kiln_01.Schedule.NumberOfSegments - 1
        ElseIf SegmentOut >= Main.Kiln_01.Schedule.NumberOfSegments Then
            SegmentOut = Main.Kiln_01.Schedule.NumberOfSegments - 1
        End If
        Main.SendModbusHoldingRegister(Main.MB_SCH_SEG_SELECTED, SegmentOut)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            MAIN.SendModbusCoil(MAIN.MB_SCH_SEG_ENABLED, True)
        Else
            MAIN.SendModbusCoil(MAIN.MB_SCH_SEG_ENABLED, False)
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            MAIN.SendModbusCoil(MAIN.MB_SCH_SEG_HOLD_EN, True)
        Else
            MAIN.SendModbusCoil(MAIN.MB_SCH_SEG_HOLD_EN, False)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        MAIN.SendModbusCoil(MAIN.MB_CMD_WRITE_EEPROM, True)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        SCHEDULE_NAME.TextBox1.Text = MAIN.Kiln_01.Schedule.Name
        SCHEDULE_NAME.Show()
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        SEGMENT_NAME.TextBox1.Text = MAIN.Kiln_01.Schedule.Segment.Name
        SEGMENT_NAME.NumericUpDown1.Value = MAIN.Kiln_01.Schedule.Segment.Setpoint
        SEGMENT_NAME.NumericUpDown2.Value = MAIN.Kiln_01.Schedule.Segment.RampRate
        SEGMENT_NAME.NumericUpDown3.Value = MAIN.Kiln_01.Schedule.Segment.SoakTime
        SEGMENT_NAME.Show()
    End Sub
End Class