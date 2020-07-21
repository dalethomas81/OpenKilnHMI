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
End Class