Imports Newtonsoft.Json
Imports System.Net
Imports System.Text
Imports System.IO

Public Class MinecraftLogin
    Dim MCName As String
    Dim MCPass As String
    Dim APIBase As String = "https://authserver.mojang.com"
    Dim Authenticate As String = APIBase & "/authenticate"
    Dim AuthTok As String
    Dim ClientTok As String
    Public Property Username As String
        Get
            Return MCName
        End Get
        Set(value As String)
            MCName = value
        End Set
    End Property
    Public WriteOnly Property Password As String
        Set(value As String)
            MCPass = value
        End Set
    End Property
    Public ReadOnly Property AuthToken As String
        Get
            Return AuthTok
        End Get
    End Property
    Public Property ClientToken As String
        Get
            Return ClientTok
        End Get
        Set(value As String)
            ClientTok = value
        End Set
    End Property

    Public Function Login()
        Dim LoginInfo As New LoginDetails
        LoginInfo.username = MCName
        LoginInfo.password = MCPass
        LoginInfo.agent.name = "Minecraft"
        LoginInfo.agent.version = "1"
        If ClientTok = "" Then
            ClientTok = GenerateUUID()
        End If
        LoginInfo.clientToken = ClientTok

        Dim LoginJSON As String = JsonConvert.SerializeObject(LoginInfo)

        Dim Result As String = SendRequest(New Uri(Authenticate), LoginJSON)
        If Result.ToString = False.ToString = False Then
            'MsgBox(Result)
            'My.Computer.Clipboard.SetText(Result)
            Dim FinalResult = JsonConvert.DeserializeObject(Of LoginResult)(Result)
            AuthTok = FinalResult.accessToken
            ClientTok = FinalResult.clientToken
            MCName = FinalResult.selectedProfile.name
            Return True
        Else
            Return False
        End If

    End Function
    Private Class SelectedProfile
        Public Property id As String
        Public Property name As String
    End Class

    Private Class AvailableProfile
        Public Property id As String
        Public Property name As String
    End Class

    Private Class LoginResult
        Public Property accessToken As String
        Public Property clientToken As String
        Public Property selectedProfile As New SelectedProfile
        Public Property availableProfiles As AvailableProfile()
    End Class
    Private Function SendRequest(uri As Uri, jsonData As String) As String
        Try
            Dim jsonDataBytes = Encoding.UTF8.GetBytes(jsonData)

            Dim req As WebRequest = WebRequest.Create(uri)
            req.ContentType = "application/json"
            req.Method = "POST"
            req.ContentLength = jsonDataBytes.Length


            Dim stream = req.GetRequestStream()
            stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
            stream.Close()

            Dim response = req.GetResponse().GetResponseStream()

            Dim reader As New StreamReader(response)
            Dim res = reader.ReadToEnd()
            reader.Close()
            response.Close()

            Return res
        Catch ex As exception
            Return False
        End Try

    End Function
    Private Function GenerateUUID()
        Dim sGUID As String
        sGUID = System.Guid.NewGuid.ToString()
        Return sGUID
    End Function

    Private Class Agent
        Public Property name As String
        Public Property version As Integer
    End Class
    Private Class LoginDetails
        Public Property agent As New Agent
        Public Property username As String
        Public Property password As String
        Public Property clientToken As String

    End Class

End Class
