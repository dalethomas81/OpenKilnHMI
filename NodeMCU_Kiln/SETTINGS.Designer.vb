<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SETTINGS
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'Button6
        '
        Me.Button6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Button6.Location = New System.Drawing.Point(12, 124)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(108, 53)
        Me.Button6.TabIndex = 28
        Me.Button6.Text = "CLOSE"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(69, 66)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(210, 17)
        Me.CheckBox1.TabIndex = 29
        Me.CheckBox1.Text = "THERMAL MONITORING OVERRIDE"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'SETTINGS
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(365, 189)
        Me.ControlBox = False
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.Button6)
        Me.Name = "SETTINGS"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Button6 As Button
    Friend WithEvents CheckBox1 As CheckBox
End Class
