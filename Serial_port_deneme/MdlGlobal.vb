Module MdlGlobal
    Public comPortStr As String
    Public comPortStr2 As String
    Sub Ayarları_Oku()
        Dim str() As String
        Dim File_Reader As System.IO.StreamReader
        File_Reader = IO.File.OpenText("ayarlar.ini")
        str = File_Reader.ReadLine.Split("=")
        comPortStr = str(1)
        str = File_Reader.ReadLine.Split("=")
        comPortStr2 = str(1)
        File_Reader.Close()
    End Sub
End Module
