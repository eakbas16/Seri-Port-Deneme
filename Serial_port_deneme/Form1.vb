Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class Form1
    Public Delegate Sub myDelegate()
    Dim seriPort2Datalistesi As ConcurrentBag(Of String) = New ConcurrentBag(Of String)()
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Call Ayarları_Oku()
        'Seri port bağlantısı için
        Try
            SerialPort1.PortName = comPortStr
            SerialPort1.BaudRate = 9600
            SerialPort1.ReceivedBytesThreshold = 2
            SerialPort1.Open()
        Catch ex As Exception
            MessageBox.Show("Seri Port bağlantısı kurulamadı")
        End Try

        Try
            SerialPort2.PortName = comPortStr2
            SerialPort2.BaudRate = 9600
            SerialPort2.ReceivedBytesThreshold = 2
            SerialPort2.Open()
        Catch ex As Exception
            MessageBox.Show("Seri Port 2 bağlantısı kurulamadı")
        End Try

        Label2.Text = Date.Now.Date

        Dim dateNow = DateTime.Now
        Dim dfi = DateTimeFormatInfo.CurrentInfo
        Dim calendar = dfi.Calendar
        Dim weekOfyear = calendar.GetWeekOfYear(
            dateNow,
            dfi.CalendarWeekRule,
            DayOfWeek.Thursday)
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf SeriPortYazdir1Thread))
    End Sub
    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Invoke(New myDelegate(AddressOf updateTextBox), New Object() {})
    End Sub
    Public Sub updateTextBox()
        Timer1.Enabled = True
    End Sub
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        Label1.BackColor = Color.Lime
        Dim rcv_byte(255) As Byte
        If SerialPort1.IsOpen = True Then
            If SerialPort1.BytesToRead > 0 Then
                Dim rcv_len As Integer = SerialPort1.Read(rcv_byte, 0, rcv_byte.Length)
                Label2.Text = rcv_len
                For i As Integer = 0 To rcv_len - 1
                    TextBox1.Text = TextBox1.Text & Chr(rcv_byte(i))
                Next
            End If
        End If


        If SerialPort1.IsOpen = True Then
            If SerialPort2.BytesToRead > 0 Then
                Dim rcv_len As Integer = SerialPort2.Read(rcv_byte, 0, rcv_byte.Length)
                Label2.Text = rcv_len
                For i As Integer = 0 To rcv_len - 1
                    TextBox1.Text = TextBox2.Text & Chr(rcv_byte(i))
                Next
            End If
        End If
        Timer2.Enabled = True
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Timer2.Enabled = False
        Label1.BackColor = SystemColors.Control
    End Sub

    Private Sub SeriPortYazdir1Thread()
basla:

tekrar:
        While Not seriPort2Datalistesi.IsEmpty
            Dim item As String = ""
            If seriPort2Datalistesi.TryTake(item) Then

                Dim sended_string As String = item
                Dim sending_byte(Len(sended_string) - 1) As Byte
                For i As Integer = 0 To sending_byte.Length - 1
                    sending_byte(i) = Asc(Mid(sended_string, i + 1, 1))
                Next
                SerialPort2.Write(sending_byte, 0, sending_byte.Length)

            End If
            Thread.Sleep(1000)
        End While
        Thread.Sleep(100)
        GoTo tekrar
    End Sub


    Private Sub SerialPort2_DataReceived(sender As Object, e As Ports.SerialDataReceivedEventArgs) Handles SerialPort2.DataReceived
        Invoke(New myDelegate(AddressOf updateTextBox), New Object() {})
    End Sub

    Private Sub btnDataGonder_Click(sender As Object, e As EventArgs) Handles btnDataGonder.Click
        Dim sended_string As String = TextBox2.Text
        Dim sending_byte(7) As Byte
        sending_byte(0) = 2
        sending_byte(1) = 48
        sending_byte(2) = 49
        sending_byte(3) = 48
        sending_byte(4) = 48
        sending_byte(5) = 67
        sending_byte(6) = 51
        sending_byte(7) = 3
        SerialPort1.Write(sending_byte, 0, sending_byte.Length)
    End Sub

    Private Sub btnEtiketYazdir_Click(sender As Object, e As EventArgs) Handles btnEtiketYazdir.Click
        Dim myString As String
        Dim yazdirStr As String = ""
        Dim value As String = File.ReadAllText("peks2.prn")
        myString = Regex.Replace(value, "123456", TextBox2.Text, RegexOptions.IgnoreCase)
        myString = Regex.Replace(myString, "emre", TextBox2.Text, RegexOptions.IgnoreCase)
        myString = Regex.Replace(myString, "654321", TextBox2.Text, RegexOptions.IgnoreCase)
        myString = Regex.Replace(myString, "999999", TextBox2.Text, RegexOptions.IgnoreCase)
        myString = Regex.Replace(myString, "1734080", TextBox2.Text, RegexOptions.IgnoreCase)
        yazdirStr &= myString
        seriPort2Datalistesi.Add(myString)
    End Sub
End Class
