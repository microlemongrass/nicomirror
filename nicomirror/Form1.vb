Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.Windows.Threading
Imports Microsoft.VisualBasic.FileIO


Public Class Form1

    Private Const GW_HWNDNEXT = 2
    '■ウィンドウを移動する。Movewindow
    Private Declare Function MoveWindow Lib "user32" Alias "MoveWindow" _
    (ByVal hwnd As IntPtr, ByVal x As Integer, ByVal y As Integer,
    ByVal nWidth As Integer, ByVal nHeight As Integer,
    ByVal bRepaint As Integer) As Integer

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function SetWindowText(hWnd As IntPtr,
        lpString As String) As Integer
    End Function
    <DllImport("user32")> Private Shared Function GetParent(ByVal hwnd As Integer) As Integer
    End Function
    <DllImport("user32")> Private Shared Function GetWindow(ByVal hwnd As Integer, ByVal wCmd As Integer) As Integer
    End Function
    <DllImport("user32")> Private Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    End Function
    <DllImport("user32")> Private Shared Function GetWindowThreadProcessId(ByVal hwnd As Integer, ByRef lpdwprocessid As Integer) As Integer
    End Function
    <DllImport("user32")> Private Shared Function IsWindowVisible(ByVal hwnd As Integer) As Integer
    End Function



    Private _cookie As CookieContainer

    Private WithEvents timmonitor As New DispatcherTimer()
    Private WithEvents timlogin As New DispatcherTimer()

    '■放送者名の一時保存
    Private name1 As String
    Private name2 As String
    Private name3 As String
    Private name4 As String
    Private name5 As String
    Private name6 As String
    Private name7 As String
    Private name8 As String
    Private name9 As String
    Private name10 As String
    Private name11 As String
    Private name12 As String
    Private name13 As String
    Private name14 As String
    Private name15 As String
    Private name16 As String

    '■lv番号一時保存
    Private temp_plat1 As String = 0
    Private temp_plat2 As String = 0
    Private temp_plat3 As String = 0
    Private temp_plat4 As String = 0
    Private temp_plat5 As String = 0
    Private temp_plat6 As String = 0
    Private temp_plat7 As String = 0
    Private temp_plat8 As String = 0
    Private temp_plat9 As String = 0
    Private temp_plat10 As String = 0
    Private temp_plat11 As String = 0
    Private temp_plat12 As String = 0
    Private temp_plat13 As String = 0
    Private temp_plat14 As String = 0
    Private temp_plat15 As String = 0
    Private temp_plat16 As String = 0

    '■準備中の画像のパス
    Private image_pass As String


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        cmbreloadinterval.SelectedIndex = 2

        With dg1

            .DefaultCellStyle.BackColor = Color.WhiteSmoke 'GhostWhite
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.Font = New Font("Meiryo UI", 10)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Meiryo UI", 9)
            .AllowUserToResizeRows = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .RowHeadersWidth = 40

        End With

        dg1.Rows(0).Selected = True

        txtpass.PasswordChar = "*"c
        txtaddress.PasswordChar = "*"c
        txtuser_session.PasswordChar = "*"c


        timmonitor.Interval = New TimeSpan(0, 0, 0, 0, numinterval_monitor.Value * 1000)


        '■表の読み込み
        Dim ii As Integer
        For ii = 0 To 0
            Try

                Dim myfilename_table1 As String = ".\data.csv"
                Dim parser As TextFieldParser = New TextFieldParser(myfilename_table1, Encoding.GetEncoding("Shift_JIS"))

                parser.TextFieldType = FieldType.Delimited
                parser.SetDelimiters(",") ' 区切り文字はコンマ

                dg1.Rows.Clear() ' データをすべてクリア

                While (Not parser.EndOfData)
                    Dim row As String() = parser.ReadFields() ' 1行読み込み
                    dg1.Rows.Add(row) ' 読み込んだデータ(1行をDataGridViewに表示する)

                End While

                For Each c As DataGridViewColumn In dg1.Columns
                    c.SortMode = DataGridViewColumnSortMode.NotSortable
                Next c

                parser.Close()


            Catch
                MsgBox("表の読み込みに失敗しました。")
            End Try

        Next

        If chklogin_auto.Checked = True Then
            btnlogin_browser.PerformClick()

        End If


        If chk2factor.Checked = True Then
            txtaddress.Enabled = False
            txtpass.Enabled = False
            txtuser_session.Enabled = True
            chklogin_auto.Checked = False
            chklogin_auto.Enabled = False

        Else
            txtaddress.Enabled = True
            txtpass.Enabled = True
            txtuser_session.Enabled = False
            chklogin_auto.Enabled = True

        End If


    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Application.DoEvents()

        Try
            '■空セルを適当に埋める
            dg1.AllowUserToAddRows = False
            Dim columncount As Integer = dg1.ColumnCount
            Dim rowcount As Integer = dg1.RowCount

            For y = 0 To rowcount - 1

                For x = 0 To columncount - 1

                    If dg1(x, y).Value = Nothing Then
                        Console.WriteLine(x & y)
                        dg1(x, y).Value = 0


                    End If

                Next

            Next


            '■表示中のデータをCSV形式で(上書き保存)保存
            Dim FileName As String = "./data.csv"
            '現在のファイルに上書き保存
            Using swCsv As New System.IO.StreamWriter(FileName, False, System.Text.Encoding.GetEncoding("SHIFT_JIS"))
                Dim sf As String = Chr(34)          'データの前側の括り
                Dim se As String = Chr(34) & ","    'データの後ろの括りとデータの区切りの "," 
                Dim i, j As Integer
                Dim WorkText As String = ""         '1個分のデータ保持用
                Dim LineText As String = ""         '1列分のデータ保持用

                With dg1

                    dg1.AllowUserToAddRows = False '最下部の新しい行（追加オプション）を非表示にする

                    '■実データ部分の取得・保存処理
                    For i = 0 To .RowCount - 1
                        LineText = ""                                         '１行分のデータをクリア
                        For j = 0 To .Columns.Count - 1                       '１行分のデータを取得処理
                            WorkText = .Item(j, i).Value.ToString              '１個セルデータを取得
                            If WorkText.IndexOf(Chr(34)) > -1 Then             'データ内に " があるか検索
                                WorkText = WorkText.Replace("""", """""")       'あれば " を "" に置換える
                            End If
                            If j = .Columns.Count - 1 Then                     '１行分の列データを連結
                                LineText &= sf & WorkText & sf                  '最後の列の場合
                            Else
                                LineText &= sf & WorkText & se
                            End If
                        Next j
                        swCsv.WriteLine(LineText)                             '1行分のデータを書き込み
                    Next i
                End With

            End Using
        Catch ex As Exception
            MsgBox("表の保存に失敗しました。")
        End Try


    End Sub

    '■プロセスID(pid)をウィンドウハンドル(hwnd)に変換する
    Public Function GetHwndFromPid(ByVal pid As Integer) As Integer

        Dim hwnd As Integer
        hwnd = FindWindow(vbNullString, vbNullString)

        Do While hwnd <> 0

            If GetParent(hwnd) = 0 And IsWindowVisible(hwnd) <> 0 Then

                If pid = GetPidFromHwnd(hwnd) Then
                    Return hwnd

                End If

            End If

            hwnd = GetWindow(hwnd, GW_HWNDNEXT)

        Loop

        Return hwnd


    End Function

    '■ウィンドウハンドル(hwnd)をプロセスID(pid)に変換する
    Public Function GetPidFromHwnd(ByVal hwnd As Integer) As Integer

        Dim pid As Integer
        Call GetWindowThreadProcessId(hwnd, pid)
        GetPidFromHwnd = pid

    End Function

    '■URLのD&D
    Private Sub dg1_DragEnter(sender As Object, e As DragEventArgs) Handles dg1.DragEnter

        If e.Data.GetDataPresent("UniformResourceLocator") Then 'URLのみ受け入れる
            e.Effect = DragDropEffects.Link

        Else
            e.Effect = DragDropEffects.None

        End If


    End Sub

    Private Sub dg1_DragDrop(sender As Object, e As DragEventArgs) Handles dg1.DragDrop
        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim aaa As Integer = dg1.SelectedRows(0).Index.ToString() '選択されている行を表示


                '■ドロップされたリンクのURLを取得する
                Dim url As String = e.Data.GetData(DataFormats.Text).ToString()
                Dim str As String = url

                '■コミュニティurlがドロップされた場合、co番号抜き取り＋lv番号検出
                If 0 <= str.IndexOf("community/co") Then
                    Dim strUrl As String 'URL
                    Dim myWebClient As New WebClient    'Web
                    Dim lefturl As String, righturl As String
                    Dim filename As String

                    strUrl = str 'URLをセット

                    filename = str
                    lefturl = InStr(filename, "community/co") + 9


                    Dim s2 As String = "" & filename.Substring(lefturl)

                    If s2.Contains("?") = True Then
                        righturl = InStr(filename, "?")
                        s2 = filename.Substring(lefturl, righturl - lefturl - 1) & ""  '4文字目から3文字を取得する

                    End If


                    If dg1.RowCount - 1 <= aaa Then '最終行を選択している状態の場合、1行追加する。
                        dg1.Rows.Add(1)
                    End If

                    dg1(dg_co.Index, aaa).Value = s2


                    Dim rowcount1 As Integer = dg1.Rows.Count - 1 '現在の行数
                    Dim selectrow As Integer

                    For Each r As DataGridViewRow In dg1.SelectedRows
                        Console.WriteLine(r.Index)
                        selectrow = r.Index
                    Next r

                    dg1.Rows(aaa).Selected = False
                    dg1.Rows(aaa + 1).Selected = True '次の行を選択する



                    '■コミュニティ情報取得
                    Dim conumber As String
                    conumber = dg1(dg_co.Index, aaa).Value
                    Console.WriteLine(conumber)

                    If 0 <= conumber.IndexOf("co") Then
                        Console.WriteLine("lv番号取得を試みます。")
                        dg_getlvnumber_first(conumber, aaa)

                    End If


                ElseIf 0 <= str.IndexOf("https://www.twitch.tv/") Then

                    If dg1.RowCount - 1 <= aaa Then '最終行を選択している状態の場合、1行追加する。
                        dg1.Rows.Add(1)

                    End If

                    Dim rowcount1 As Integer = dg1.Rows.Count - 1
                    Dim selectrow As Integer

                    For Each r As DataGridViewRow In dg1.SelectedRows
                        Console.WriteLine(r.Index)
                        selectrow = r.Index
                    Next r

                    dg1.Rows(aaa).Selected = False
                    dg1.Rows(aaa + 1).Selected = True '次の行を選択する

                    dg1(dg_co.Index, aaa).Value = "Twitch"

                    Dim replaceClip As String = ""

                    replaceClip = str.Replace("https://www.twitch.tv/", "https://player.twitch.tv/?channel=")
                    dg1(dg_lv.Index, aaa).Value = replaceClip

                    If dg1(dg_name.Index, aaa).Value = "" Or dg1(dg_name.Index, aaa).Value = "0" Then
                        Dim usernameTwitch As String = str.Replace("https://www.twitch.tv/", "")

                        dg1(dg_name.Index, aaa).Value = usernameTwitch

                    End If

                    dg1.FirstDisplayedScrollingRowIndex = dg1.Rows.Count - 1 '最下までスクロール







                ElseIf 0 <= str.IndexOf("https://www.youtube.com/watch?v=") Then

                    '最終行を選択している状態の場合、1行追加する。
                    If dg1.RowCount - 1 <= aaa Then
                        dg1.Rows.Add(1)
                    End If


                    Dim rowcount1 As Integer = dg1.Rows.Count - 1 '現在の行数
                    Dim selectrow As Integer

                    For Each r As DataGridViewRow In dg1.SelectedRows
                        Console.WriteLine(r.Index)
                        selectrow = r.Index
                    Next r

                    dg1.Rows(aaa).Selected = False '次の行を選択する

                    dg1.Rows(aaa + 1).Selected = True '次の行を選択する


                    dg1(dg_co.Index, aaa).Value = "Youtube"
                    Dim replaceClip As String = ""

                    replaceClip = str.Replace("https://www.youtube.com/watch?v=", "https://www.youtube.com/embed/")
                    dg1(dg_lv.Index, aaa).Value = replaceClip

                    dg1.FirstDisplayedScrollingRowIndex = dg1.Rows.Count - 1 '最下までスクロール





                ElseIf 0 <= str.IndexOf("https://mixer.com/") Then
                    '最終行を選択している状態の場合、1行追加する。
                    If dg1.RowCount - 1 <= aaa Then
                        dg1.Rows.Add(1)
                    End If


                    Dim rowcount1 As Integer = dg1.Rows.Count - 1 '現在の行数
                    Dim selectrow As Integer

                    For Each r As DataGridViewRow In dg1.SelectedRows
                        Console.WriteLine(r.Index)
                        selectrow = r.Index
                    Next r

                    dg1.Rows(aaa).Selected = False '次の行を選択する

                    dg1.Rows(aaa + 1).Selected = True '次の行を選択する


                    dg1(dg_co.Index, aaa).Value = "Mixer"
                    Dim replaceClip As String = ""

                    replaceClip = str.Replace("https://mixer.com/", "https://mixer.com/embed/player/")
                    dg1(dg_lv.Index, aaa).Value = replaceClip

                    If dg1(dg_name.Index, aaa).Value = "" Or dg1(dg_name.Index, aaa).Value = "0" Then
                        Dim usernameMixer As String = str.Replace("https://mixer.com/", "")

                        dg1(dg_name.Index, aaa).Value = usernameMixer

                    End If

                    dg1.FirstDisplayedScrollingRowIndex = dg1.Rows.Count - 1 '最下までスクロール

                End If

            End If

        Catch ex As Exception
            MsgBox("えらー")
        End Try


    End Sub



    '■配信情報の取得
    Private Sub btnlogin_browser_Click(sender As Object, e As EventArgs) Handles btnlogin_browser.Click

        Try
            getcookie()

        Catch
        End Try

    End Sub

    Private Sub getcookie()

        If chk2factor.Checked = False Then

            Dim cc As CookieContainer = NicoVideoAPI.Login(txtaddress.Text, txtpass.Text)
            _cookie = cc

            Dim api As String = NicoVideoAPI.Read(_cookie, "http://watch.live.nicovideo.jp/api/getplayerstatus?v=co1")
            rtxthtml.Text = api

            If 0 <= api.IndexOf("<code>notlogin</code>") Then
                Console.WriteLine("クッキー取得失敗")
                rtxtconsole.AppendText(DateTime.Now & "  Cookieの取得に失敗しました。" & vbCrLf)

            Else
                Console.WriteLine("クッキー取得成功")
                rtxtconsole.AppendText(DateTime.Now & "  Cookieの取得に成功しました。" & vbCrLf)

                tsscookie.Text = "Cookie：◯"
                chklogin_auto.Checked = True
                btngetlv_reload.PerformClick()

            End If


        ElseIf chk2factor.Checked = True Then
            Dim userSession As String = txtuser_session.Text '"user_session_xxxxx"
            Dim wc As New WebClient
            Dim enc As Encoding = Encoding.UTF8

            wc.Headers.Add("Cookie", "user_session=" & userSession)

            Dim data As Byte() = wc.DownloadData("https://api.ce.nicovideo.jp/api/v1/session.create")
            Dim res As String = enc.GetString(data)

            If 0 <= res.IndexOf("<code>notlogin</code>") Then
                Console.WriteLine("クッキー取得失敗")
                rtxtconsole.AppendText(DateTime.Now & "  Cookieの取得に失敗しました。" & vbCrLf)

            Else
                Console.WriteLine("クッキー取得成功")
                rtxtconsole.AppendText(DateTime.Now & "  Cookieの取得に成功しました。" & vbCrLf)

                tsscookie.Text = "Cookie：◯"
                btngetlv_reload.PerformClick()

            End If

        End If


    End Sub

    Private Sub btngetlv_reload_Click(sender As Object, e As EventArgs) Handles btngetlv_reload.Click

        Try
            dg1.AllowUserToAddRows = False
            Dim rowcount1 As Integer = dg1.Rows.Count

            For i = 0 To rowcount1 - 1
                Dim conumber As String = dg1(dg_co.Index, i).Value

                If 0 <= conumber.IndexOf("co") Then
                    dg_getlvnumber(conumber, i)

                End If

            Next

            dg1.AllowUserToAddRows = True

            rtxtconsole.AppendText(DateTime.Now & "  最新情報に更新されました。" & vbCrLf)
            tsslatestupdate.Text = "     最終更新：" & DateTime.Now

        Catch
        End Try


    End Sub


    Private Async Sub dg_getlvnumber(coxx As String, dgcount As Integer)

        Console.WriteLine("lv番号取得を試みます : " & coxx)

        Dim str_lv As String = coxx
        Dim _title As String = ""
        Dim _id As String = ""
        Dim _name As String = ""

        '■コミュニティURL加工後が入っているので、URL補完＆lv番号取得
        If 0 <= str_lv.IndexOf("co") Then

            '■放送情報を取得する
            dg1(dg_title.Index, dgcount).Value = ""
            dg1(dg_lv.Index, dgcount).Value = "-"

            If dg1(dg_name.Index, dgcount).Value = "" Then
                dg1(dg_name.Index, dgcount).Value = "-"

            End If

            Await Task.Run(
                Sub()

                    Dim api As String

                    If chk2factor.Checked = False Then
                        api = NicoVideoAPI.Read(_cookie, "http://watch.live.nicovideo.jp/api/getplayerstatus?v=" & coxx)

                    ElseIf chk2factor.Checked = True Then
                        Dim userSession As String = txtuser_session.Text '"user_session_xxxxx"
                        Dim wc As New WebClient
                        Dim enc As Encoding = Encoding.UTF8

                        wc.Headers.Add("Cookie", "user_session=" & userSession)

                        Dim data As Byte() = wc.DownloadData("http://watch.live.nicovideo.jp/api/getplayerstatus?v=" & coxx)

                        api = enc.GetString(data)

                    End If

                    '■タイトル(title)、放送lv(id)、放送者(owner_name)を取得
                    Dim reg_title As String = "<title>*>(?<text>[^<]*)</title>"    '"<a\\s+[^>]*href\\s*=\\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>"
                    Dim text_title As String = api
                    Dim r_title As Regex = New Regex(reg_title, RegexOptions.IgnoreCase)
                    Dim collection_title = r_title.Matches(text_title)

                    dg1.Rows(dgcount).DefaultCellStyle.BackColor = Color.WhiteSmoke

                    For Each m_title As Match In collection_title ' マッチした情報を出力
                        Console.WriteLine("タイトル：" & $"{m_title.Groups("text").Value.Trim()}")
                        _title = $"{m_title.Groups("text").Value.Trim()}"

                    Next


                    '■放送lv(id)にマッチする正規表現
                    Dim reg_id As String = "<id>*>(?<text>[^<]*)</id>"    '"<a\\s+[^>]*href\\s*=\\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>"
                    Dim text_id As String = api
                    Dim r_id As Regex = New Regex(reg_id, RegexOptions.IgnoreCase)
                    Dim collection_id = r_id.Matches(text_id)

                    For Each m_id As Match In collection_id ' マッチした情報を出力
                        Console.WriteLine("放送lv：" & $"{m_id.Groups("text").Value.Trim()}")
                        _id = $"{m_id.Groups("text").Value.Trim()}"

                    Next


                    '■放送者(owner_name)にマッチする正規表現
                    Dim reg_name As String = "<owner_name>*>(?<text>[^<]*)</owner_name>"    '"<a\\s+[^>]*href\\s*=\\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>"
                    Dim text_name As String = api
                    Dim r_name As Regex = New Regex(reg_name, RegexOptions.IgnoreCase)
                    Dim collection_name = r_name.Matches(text_name)

                    For Each m_name As Match In collection_name
                        ' マッチした情報を出力
                        Console.WriteLine("放送者：" & $"{m_name.Groups("text").Value.Trim()}")
                        _name = $"{m_name.Groups("text").Value.Trim()}"

                    Next

                End Sub)

            dg1(dg_title.Index, dgcount).Value = _title
            dg1(dg_lv.Index, dgcount).Value = _id

            If dg1(dg_name.Index, dgcount).Value = "-" Then
                dg1(dg_name.Index, dgcount).Value = _name

            End If


            If 0 <= dg1(dg_lv.Index, dgcount).Value.IndexOf("lv") Then
                dg1.Rows(dgcount).DefaultCellStyle.BackColor = Color.FromArgb(255, 239, 213)

            End If

        End If


    End Sub

    Private Sub dg_getlvnumber_first(coxx As String, dgcount As Integer)

        Dim str_lv As String = coxx

        '■コミュニティURL加工後が入っているので、URL補完＆lv番号取得
        If 0 <= str_lv.IndexOf("co") Then

            Dim api As String

            If chk2factor.Checked = False Then
                api = NicoVideoAPI.Read(_cookie, "http://watch.live.nicovideo.jp/api/getplayerstatus?v=" & coxx)

            ElseIf chk2factor.Checked = True Then
                Dim userSession As String = txtuser_session.Text '"user_session_xxxxx"
                Dim wc As New WebClient
                Dim enc As Encoding = Encoding.UTF8

                wc.Headers.Add("Cookie", "user_session=" & userSession)

                Dim data As Byte() = wc.DownloadData("http://watch.live.nicovideo.jp/api/getplayerstatus?v=" & coxx)

                api = enc.GetString(data)

            End If

            '■情報取得
            dg1(dg_title.Index, dgcount).Value = ""
            dg1(dg_lv.Index, dgcount).Value = "-"

            If dg1(dg_name.Index, dgcount).Value = "" Then
                dg1(dg_name.Index, dgcount).Value = "-"

            End If

            Dim _title As String = ""
            Dim _id As String = ""
            Dim _name As String = ""

            '■タイトル(title)、放送lv(id)、放送者(owner_name)を取得
            Dim reg_title As String = "<title>*>(?<text>[^<]*)</title>"    '"<a\\s+[^>]*href\\s*=\\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>"
            Dim text_title As String = api
            Dim r_title As Regex = New Regex(reg_title, RegexOptions.IgnoreCase)
            Dim collection_title = r_title.Matches(text_title)

            dg1.Rows(dgcount).DefaultCellStyle.BackColor = Color.WhiteSmoke

            For Each m_title As Match In collection_title                ' マッチした情報を出力
                Console.WriteLine("タイトル：" & $"{m_title.Groups("text").Value.Trim()}")
                _title = $"{m_title.Groups("text").Value.Trim()}"

                dg1.Rows(dgcount).DefaultCellStyle.BackColor = Color.FromArgb(255, 239, 213)

            Next


            '■放送lv(id)にマッチする正規表現
            Dim reg_id As String = "<id>*>(?<text>[^<]*)</id>"    '"<a\\s+[^>]*href\\s*=\\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>"
            Dim text_id As String = api
            Dim r_id As Regex = New Regex(reg_id, RegexOptions.IgnoreCase)
            Dim collection_id = r_id.Matches(text_id)

            For Each m_id As Match In collection_id ' マッチした情報を出力
                Console.WriteLine("放送lv：" & $"{m_id.Groups("text").Value.Trim()}")
                _id = $"{m_id.Groups("text").Value.Trim()}"

            Next


            '■放送者(owner_name)にマッチする正規表現
            Dim reg_name As String = "<owner_name>*>(?<text>[^<]*)</owner_name>"    '"<a\\s+[^>]*href\\s*=\\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>"
            Dim text_name As String = api
            Dim r_name As Regex = New Regex(reg_name, RegexOptions.IgnoreCase)
            Dim collection_name = r_name.Matches(text_name)

            For Each m_name As Match In collection_name                ' マッチした情報を出力
                Console.WriteLine("放送者：" & $"{m_name.Groups("text").Value.Trim()}")
                _name = $"{m_name.Groups("text").Value.Trim()}"

            Next

            dg1(dg_title.Index, dgcount).Value = _title
            dg1(dg_lv.Index, dgcount).Value = _id
            dg1(dg_name.Index, dgcount).Value = _name

        End If


    End Sub


    Private Sub numinterval_monitor_ValueChanged(sender As Object, e As EventArgs) Handles numinterval_monitor.ValueChanged

        timmonitor.Interval = New TimeSpan(0, 0, 0, 0, numinterval_monitor.Value * 1000)


    End Sub

    Private Sub timmonitor_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles timmonitor.Tick

        btngetlv_reload.PerformClick()


    End Sub


    '■表の並べ替え
    Private Sub btnascending_order_Click(sender As Object, e As EventArgs) Handles btnascending_order.Click

        dg1.Sort(New CustomComparer(SortOrder.Ascending)) '昇順で並び替えを行う


    End Sub

    '■アドレス/パスの表示、非表示
    Private Sub btnmailview_Click(sender As Object, e As EventArgs) Handles btnmailview.Click

        If txtaddress.PasswordChar = "*"c Then
            txtaddress.PasswordChar = ""

        Else
            txtaddress.PasswordChar = "*"c

        End If


    End Sub

    Private Sub btnpassview_Click(sender As Object, e As EventArgs) Handles btnpassview.Click

        If txtpass.PasswordChar = "*"c Then
            txtpass.PasswordChar = ""

        Else
            txtpass.PasswordChar = "*"c

        End If


    End Sub


    Private Sub chkmonitor_CheckedChanged(sender As Object, e As EventArgs) Handles chkmonitor.CheckedChanged

        timmonitor.Interval = New TimeSpan(0, 0, 0, 0, numinterval_monitor.Value * 1000)
        timmonitor.IsEnabled = chkmonitor.Checked


    End Sub







    '■配信を開くボタン
    Private Sub btnweb1_Click(sender As Object, e As EventArgs) Handles btnweb1.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name1 = dg1(dg_name.Index, rowcount).Value
                btnweb1.Text = name1

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(0))

                SetWindowText(Handle, "name_" & name1)
                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat1 = "nico"
                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound1.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound1.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat1 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound1.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound1.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat1 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound1.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound1.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat1 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound1.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound1.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")


                End If


            Else
                '■blank
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(0))
                SetWindowText(Handle, "lv_blank_")


            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb2_Click(sender As Object, e As EventArgs) Handles btnweb2.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name2 = dg1(dg_name.Index, rowcount).Value
                btnweb2.Text = name2

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(1))
                SetWindowText(Handle, "name_" & name2)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat2 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound2.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound2.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat2 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound2.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound2.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat2 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound2.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound2.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat2 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound2.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound2.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")


                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(1))
                '■blank
                SetWindowText(Handle, "lv_blank_")


            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb3_Click(sender As Object, e As EventArgs) Handles btnweb3.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index

                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value
                name3 = dg1(dg_name.Index, rowcount).Value
                btnweb3.Text = name3

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))
                SetWindowText(Handle, "name_" & name3)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat3 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound3.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound3.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat3 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound3.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound3.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat3 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound3.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound3.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat3 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound3.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound3.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb4_Click(sender As Object, e As EventArgs) Handles btnweb4.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name4 = dg1(dg_name.Index, rowcount).Value
                btnweb4.Text = name4

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))
                SetWindowText(Handle, "name_" & name4)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat4 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound4.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound4.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat4 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound4.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound4.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat4 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound4.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound4.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat4 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound4.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound4.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))

                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb5_Click(sender As Object, e As EventArgs) Handles btnweb5.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name5 = dg1(dg_name.Index, rowcount).Value
                btnweb5.Text = name5

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))
                SetWindowText(Handle, "name_" & name5)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat5 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound5.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound5.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat5 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound5.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound5.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat5 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound5.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound5.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat5 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound5.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound5.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))

                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb6_Click(sender As Object, e As EventArgs) Handles btnweb6.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name6 = dg1(dg_name.Index, rowcount).Value
                btnweb6.Text = name6

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))
                SetWindowText(Handle, "name_" & name6)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat6 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound6.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound6.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat6 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound6.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound6.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat6 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound6.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound6.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat6 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound6.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound6.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb7_Click(sender As Object, e As EventArgs) Handles btnweb7.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name7 = dg1(dg_name.Index, rowcount).Value
                btnweb7.Text = name7

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))
                SetWindowText(Handle, "name_" & name7)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat7 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound7.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound7.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat7 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound7.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound7.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat7 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound7.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound7.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat7 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound7.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound7.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb8_Click(sender As Object, e As EventArgs) Handles btnweb8.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name8 = dg1(dg_name.Index, rowcount).Value
                btnweb8.Text = name8

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))
                SetWindowText(Handle, "name_" & name8)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat8 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound8.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound8.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat8 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound8.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound8.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat8 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound8.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound8.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat8 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound8.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound8.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb9_Click(sender As Object, e As EventArgs) Handles btnweb9.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name9 = dg1(dg_name.Index, rowcount).Value
                btnweb9.Text = name9

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))
                SetWindowText(Handle, "name_" & name9)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat9 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound9.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound9.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat9 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound9.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound9.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat9 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound9.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound9.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat9 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound9.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound9.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb10_Click(sender As Object, e As EventArgs) Handles btnweb10.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name10 = dg1(dg_name.Index, rowcount).Value
                btnweb10.Text = name10

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))
                SetWindowText(Handle, "name_" & name10)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat10 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound10.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound10.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat10 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound10.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound10.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat10 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound10.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound10.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat10 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound10.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound10.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb11_Click(sender As Object, e As EventArgs) Handles btnweb11.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name11 = dg1(dg_name.Index, rowcount).Value
                btnweb11.Text = name11

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))
                SetWindowText(Handle, "name_" & name11)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat11 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound11.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound11.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat11 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound11.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound11.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat11 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound11.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound11.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat11 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound11.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound11.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb12_Click(sender As Object, e As EventArgs) Handles btnweb12.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name12 = dg1(dg_name.Index, rowcount).Value
                btnweb12.Text = name12

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))
                SetWindowText(Handle, "name_" & name12)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat12 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound12.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound12.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat12 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound12.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound12.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat12 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound12.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound12.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat12 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound12.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound12.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb13_Click(sender As Object, e As EventArgs) Handles btnweb13.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name13 = dg1(dg_name.Index, rowcount).Value
                btnweb13.Text = name13

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))
                SetWindowText(Handle, "name_" & name13)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat13 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound13.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound13.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat13 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound13.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound13.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat13 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound13.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound13.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat13 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound13.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound13.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb14_Click(sender As Object, e As EventArgs) Handles btnweb14.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name14 = dg1(dg_name.Index, rowcount).Value
                btnweb14.Text = name14

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))
                SetWindowText(Handle, "name_" & name14)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat14 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound14.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound14.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat14 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound14.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound14.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat14 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound14.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound14.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat14 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)


                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound14.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound14.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb15_Click(sender As Object, e As EventArgs) Handles btnweb15.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name15 = dg1(dg_name.Index, rowcount).Value
                btnweb15.Text = name15

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))
                SetWindowText(Handle, "name_" & name15)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)
                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat15 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound15.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound15.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat15 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound15.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound15.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat15 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound15.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound15.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat15 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound15.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound15.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub

    Private Sub btnweb16_Click(sender As Object, e As EventArgs) Handles btnweb16.Click

        Try
            Dim selectedRowCount As Integer = dg1.Rows.GetRowCount(DataGridViewElementStates.Selected)

            '■1行のみ選択されている時、co番号を取得する。dg1.SelectedCells(0).RowIndex
            If selectedRowCount = 1 Then
                Dim rowcount As Integer = dg1.CurrentRow.Index
                Dim lvxx As String = dg1(dg_lv.Index, rowcount).Value

                name16 = dg1(dg_name.Index, rowcount).Value
                btnweb16.Text = name16

                '■名前を送信
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))
                SetWindowText(Handle, "name_" & name16)

                Console.WriteLine(rowcount & ", " & lvxx)

                If chkclip.Checked = True Then
                    Clipboard.SetText(lvxx)

                End If

                Threading.Thread.Sleep(250)

                If 0 <= lvxx.IndexOf("lv") And 0 > lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat16 = "nico"

                    '■lv送信
                    SetWindowText(Handle, "lv_nico_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_nico.Text = "ON" Then
                        btnsound16.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_nico.Text = "OFF" Then
                        btnsound16.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("twitch") And 0 > lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat16 = "twitch"

                    '■lv送信
                    SetWindowText(Handle, "lv_twitch_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_twitch.Text = "ON" Then
                        btnsound16.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_twitch.Text = "OFF" Then
                        btnsound16.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("youtube") And 0 > lvxx.IndexOf("mixer") Then
                    temp_plat16 = "youtube"

                    '■lv送信
                    SetWindowText(Handle, "lv_youtube_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_youtube.Text = "ON" Then
                        btnsound16.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_youtube.Text = "OFF" Then
                        btnsound16.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                ElseIf 0 <= lvxx.IndexOf("mixer") Then
                    temp_plat16 = "mixer"

                    '■lv送信
                    SetWindowText(Handle, "lv_mixer_" & lvxx)
                    Threading.Thread.Sleep(250)

                    '■サウンドの色を調整。
                    If lblsound_mixer.Text = "ON" Then
                        btnsound16.BackColor = Color.Gold
                        SetWindowText(Handle, "chksound_on")

                    ElseIf lblsound_mixer.Text = "OFF" Then
                        btnsound16.BackColor = Color.LightGray
                        SetWindowText(Handle, "chksound_off")

                    End If


                Else
                    'コンフィグ閲覧用
                    '■lv送信
                    SetWindowText(Handle, "lv_config_")

                End If


            Else
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))
                '■blank
                SetWindowText(Handle, "lv_blank_")

            End If

        Catch ex As Exception
        End Try


    End Sub





    '■ミュートボタン
    Private Sub btnsound1_Click(sender As Object, e As EventArgs) Handles btnsound1.Click
        '■音声切り替え

        'nico
        If temp_plat1 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(0))

            '■sound信号を送信
            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound1.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound1.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound1.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound1.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat1 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(0))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound1.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound1.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound1.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound1.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")


            End If
            Exit Sub


            'twitch
        ElseIf temp_plat1 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(0))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound1.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound1.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")

            ElseIf btnsound1.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound1.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat1 = "mixer" Then

            If btnsound1.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound1.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound1.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound1.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound2_Click(sender As Object, e As EventArgs) Handles btnsound2.Click
        '■音声切り替え

        'nico
        If temp_plat2 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(1))

            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound2.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound2.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound2.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound2.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat2 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(1))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound2.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound2.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound2.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound2.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat2 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(1))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound2.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound2.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound2.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound2.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat2 = "mixer" Then

            If btnsound2.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound2.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound2.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound2.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound3_Click(sender As Object, e As EventArgs) Handles btnsound3.Click
        '■音声切り替え

        'nico
        If temp_plat3 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound3.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound3.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound3.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound3.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat3 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound3.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound3.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound3.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound3.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat3 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound3.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound3.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound3.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound3.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat3 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))

            If btnsound3.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound3.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound3.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound3.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound4_Click(sender As Object, e As EventArgs) Handles btnsound4.Click

        'nico
        If temp_plat4 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound4.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound4.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound4.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound4.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat4 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound4.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound4.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound4.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound4.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat4 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound4.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound4.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound4.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound4.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat4 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))

            If btnsound4.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound4.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound4.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound4.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound5_Click(sender As Object, e As EventArgs) Handles btnsound5.Click

        'nico
        If temp_plat5 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound5.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound5.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound5.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound5.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat5 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound5.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound5.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound5.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound5.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat5 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound5.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound5.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound5.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound5.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat5 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))

            If btnsound5.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound5.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound5.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound5.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound6_Click(sender As Object, e As EventArgs) Handles btnsound6.Click

        'nico
        If temp_plat6 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound6.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound6.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound6.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound6.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat6 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound6.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound6.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound6.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound6.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat6 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound6.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound6.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound6.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound6.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat6 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))

            If btnsound6.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound6.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound6.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound6.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound7_Click(sender As Object, e As EventArgs) Handles btnsound7.Click

        'nico
        If temp_plat7 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound7.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound7.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound7.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound7.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat7 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound7.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound7.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound7.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound7.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat7 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound7.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound7.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound7.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound7.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat7 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))

            If btnsound7.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound7.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound7.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound7.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound8_Click(sender As Object, e As EventArgs) Handles btnsound8.Click

        'nico
        If temp_plat8 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound8.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound8.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound8.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound8.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat8 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound8.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound8.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound8.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound8.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat8 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound8.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound8.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound8.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound8.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat8 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))

            If btnsound8.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound8.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound8.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound8.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound9_Click(sender As Object, e As EventArgs) Handles btnsound9.Click

        'nico
        If temp_plat9 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound9.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound9.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound9.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound9.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat9 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound9.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound9.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound9.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound9.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat9 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound9.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound9.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound9.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound9.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat9 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))

            If btnsound9.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound9.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound9.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound9.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound10_Click(sender As Object, e As EventArgs) Handles btnsound10.Click

        'nico
        If temp_plat10 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound10.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound10.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound10.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound10.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat10 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound10.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound10.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound10.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound10.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat10 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound10.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound10.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound10.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound10.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat10 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))

            If btnsound10.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound10.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound10.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound10.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound11_Click(sender As Object, e As EventArgs) Handles btnsound11.Click

        'nico
        If temp_plat11 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound11.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound11.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound11.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound11.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat11 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound11.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound11.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound11.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound11.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat11 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound11.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound11.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound11.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound11.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat11 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))

            If btnsound11.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound11.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound11.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound11.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound12_Click(sender As Object, e As EventArgs) Handles btnsound12.Click

        'nico
        If temp_plat12 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound12.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound12.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound12.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound12.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat12 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound12.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound12.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound12.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound12.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat12 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound12.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound12.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound12.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound12.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat12 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))

            If btnsound12.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound12.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound12.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound12.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound13_Click(sender As Object, e As EventArgs) Handles btnsound13.Click

        'nico
        If temp_plat13 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound13.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound13.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound13.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound13.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat13 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound13.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound13.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound13.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound13.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat13 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound13.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound13.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound13.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound13.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat13 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))

            If btnsound13.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound13.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound13.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound13.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound14_Click(sender As Object, e As EventArgs) Handles btnsound14.Click

        'nico
        If temp_plat14 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound14.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound14.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound14.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound14.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat14 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound14.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound14.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound14.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound14.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat14 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound14.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound14.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound14.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound14.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat14 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))

            If btnsound14.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound14.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound14.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound14.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound15_Click(sender As Object, e As EventArgs) Handles btnsound15.Click

        'nico
        If temp_plat15 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound15.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound15.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")

            ElseIf btnsound15.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound15.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat15 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound15.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound15.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound15.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound15.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat15 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound15.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound15.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")

            ElseIf btnsound15.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound15.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat15 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))

            If btnsound15.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound15.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound15.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound15.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub

    Private Sub btnsound16_Click(sender As Object, e As EventArgs) Handles btnsound16.Click

        'nico
        If temp_plat16 = "nico" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))
            '■sound信号を送信

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound16.BackColor = Color.Gold Then
                lblsound_nico.Text = "OFF"
                btnsound16.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_nico_off")


            ElseIf btnsound16.BackColor = Color.LightGray Then
                lblsound_nico.Text = "ON"
                btnsound16.BackColor = Color.Gold
                SetWindowText(Handle, "sound_nico_on")

            End If


            ''youtube
        ElseIf temp_plat16 = "youtube" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound16.BackColor = Color.Gold Then
                lblsound_youtube.Text = "OFF"
                btnsound16.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_youtube_off")


            ElseIf btnsound16.BackColor = Color.LightGray Then
                lblsound_youtube.Text = "ON"
                btnsound16.BackColor = Color.Gold
                SetWindowText(Handle, "sound_youtube_on")

            End If
            Exit Sub


            'twitch
        ElseIf temp_plat16 = "twitch" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))

            Console.WriteLine("音声のON/OFFを切り替えました。")

            If btnsound16.BackColor = Color.Gold Then
                lblsound_twitch.Text = "OFF"
                btnsound16.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_twitch_off")


            ElseIf btnsound16.BackColor = Color.LightGray Then
                lblsound_twitch.Text = "ON"
                btnsound16.BackColor = Color.Gold
                SetWindowText(Handle, "sound_twitch_on")

            End If
            Exit Sub


            'mixer
        ElseIf temp_plat16 = "mixer" Then
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))

            If btnsound16.BackColor = Color.Gold Then
                lblsound_mixer.Text = "OFF"
                btnsound16.BackColor = Color.LightGray
                SetWindowText(Handle, "sound_mixer_off")


            ElseIf btnsound16.BackColor = Color.LightGray Then
                lblsound_mixer.Text = "ON"
                btnsound16.BackColor = Color.Gold
                SetWindowText(Handle, "sound_mixer_on")

            End If

        End If


    End Sub




    '■リロードボタン
    Private Sub btnreload1_Click(sender As Object, e As EventArgs) Handles btnreload1.Click

        Try
            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(0))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload2_Click(sender As Object, e As EventArgs) Handles btnreload2.Click

        Try
            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(1))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload3_Click(sender As Object, e As EventArgs) Handles btnreload3.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(2))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload4_Click(sender As Object, e As EventArgs) Handles btnreload4.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(3))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload5_Click(sender As Object, e As EventArgs) Handles btnreload5.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(4))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload6_Click(sender As Object, e As EventArgs) Handles btnreload6.Click
        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(5))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload7_Click(sender As Object, e As EventArgs) Handles btnreload7.Click
        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(6))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")
            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")
            End If

        Catch ex As Exception

        End Try



    End Sub

    Private Sub btnreload8_Click(sender As Object, e As EventArgs) Handles btnreload8.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(7))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload9_Click(sender As Object, e As EventArgs) Handles btnreload9.Click
        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(8))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload10_Click(sender As Object, e As EventArgs) Handles btnreload10.Click
        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(9))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload11_Click(sender As Object, e As EventArgs) Handles btnreload11.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(10))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload12_Click(sender As Object, e As EventArgs) Handles btnreload12.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(11))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload13_Click(sender As Object, e As EventArgs) Handles btnreload13.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(12))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload14_Click(sender As Object, e As EventArgs) Handles btnreload14.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(13))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload15_Click(sender As Object, e As EventArgs) Handles btnreload15.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(14))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub

    Private Sub btnreload16_Click(sender As Object, e As EventArgs) Handles btnreload16.Click

        Try

            '■sound信号を送信
            Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(15))

            If btnswitchreload.Text = "配信画面" Then
                SetWindowText(Handle, "reload_screen_")


            ElseIf btnswitchreload.Text = "ブラウザ" Then
                SetWindowText(Handle, "reload_browser_")

            End If

        Catch
        End Try


    End Sub


    Private Sub chkallreload_CheckedChanged(sender As Object, e As EventArgs) Handles chkallreload.CheckedChanged

        If chkallreload.Checked = True Then
            timallreload.Interval = 1000 * CInt(cmbreloadinterval.SelectedItem)
            timallreload.Enabled = True
            cmbreloadinterval.Enabled = False

        Else
            timallreload.Enabled = False
            cmbreloadinterval.Enabled = True

        End If


    End Sub

    Private Sub timallreload_Tick(sender As Object, e As EventArgs) Handles timallreload.Tick

        btnreloadall.PerformClick()


    End Sub





    '■(現在未使用)埋め込みコードの整形
    Private Sub btnurlcrop_Click(sender As Object, e As EventArgs) Handles btnurlcrop.Click

        Dim rowcount As Integer = dg1.Rows.Count - 1 '現在の行数
        Dim bef_url As String

        For i = 0 To rowcount - 1
            bef_url = dg1(dg_lv.Index, i).Value
            '■Twitch
            If 0 <= bef_url.IndexOf("<iframe src=") Then
                Dim lefturl As String, righturl As String

                lefturl = InStr(bef_url, "<iframe src=") + 12

                Dim s2 As String = "" & bef_url.Substring(lefturl)

                If s2.Contains(""" frameborder") = True Then
                    righturl = InStr(bef_url, """ frameborder")
                    s2 = bef_url.Substring(lefturl, righturl - lefturl - 1) & ""  '4文字目から3文字を取得する
                    Console.WriteLine(s2)
                    dg1(dg_lv.Index, i).Value = s2

                End If


            End If


            '■Youtube
            If 0 <= bef_url.IndexOf("<iframe width=""560"" height=""315"" src=") Then
                Dim lefturl As String, righturl As String

                lefturl = InStr(bef_url, "<iframe width=""560"" height=""315"" src=") + 37

                Dim s2 As String = "" & bef_url.Substring(lefturl)

                If s2.Contains(""" frameborder") = True Then
                    righturl = InStr(bef_url, """ frameborder")
                    s2 = bef_url.Substring(lefturl, righturl - lefturl - 1) & ""  '4文字目から3文字を取得する
                    Console.WriteLine(s2)
                    dg1(dg_lv.Index, i).Value = s2

                End If

            End If


            '■Mixer
            If 0 <= bef_url.IndexOf("<iframe allowfullscreen=""true"" src=") Then
                Dim lefturl As String, righturl As String

                lefturl = InStr(bef_url, "<iframe allowfullscreen=""true"" src=") + 35

                Dim s2 As String = "" & bef_url.Substring(lefturl)

                If s2.Contains("""></iframe>") = True Then
                    righturl = InStr(bef_url, """></iframe>")
                    s2 = bef_url.Substring(lefturl, righturl - lefturl - 1) & ""  '4文字目から3文字を取得する
                    Console.WriteLine(s2)
                    dg1(dg_lv.Index, i).Value = s2

                End If

            End If

        Next


    End Sub

    '■rtxtconsoleのスクロール
    Private Sub rtxtconsole_TextChanged(sender As Object, e As EventArgs) Handles rtxtconsole.TextChanged

        rtxtconsole.SelectionStart = rtxtconsole.Text.Length 'カレット位置を末尾に移動
        rtxtconsole.Focus() 'テキストボックスにフォーカスを移動
        rtxtconsole.ScrollToCaret() 'カレット位置までスクロール


    End Sub

    '■ブラウザリロード/配信画面リロードの切り替え
    Private Sub btnswitchreload_Click(sender As Object, e As EventArgs) Handles btnswitchreload.Click

        If btnswitchreload.Text = "配信画面" Then
            btnswitchreload.Text = "ブラウザ"


        ElseIf btnswitchreload.Text = "ブラウザ" Then
            btnswitchreload.Text = "配信画面"

        End If

    End Sub

    '■ウィンドウの整列
    Private Sub btnAlignment_Click(sender As Object, e As EventArgs) Handles btnAlignment.Click

        Try

            Dim posx(260) As Integer
            Dim posy(260) As Integer
            Dim count = 0

            For yy = 0 To numval_y.Value - 1

                For xx = 0 To numval_x.Value - 1
                    posx(count) = numx.Value + xx * numwid.Value
                    posy(count) = numy.Value + yy * numhei.Value + yy * numheight.Value
                    Console.WriteLine(posx(count) & "," & posy(count))

                    count += 1

                Next

                If count = numval_x.Value * numval_y.Value Then
                    Exit For

                End If

            Next


            Dim count_pid As Integer = listbrowser.Items.Count

            For i = 0 To count_pid - 1
                Dim movex As Integer = posx(i)
                Dim movey As Integer = posy(i)
                Dim Handle As IntPtr = GetHwndFromPid(listbrowser.Items(i))

                MoveWindow(Handle, movex, movey, numwid.Value, numhei.Value + numheight.Value, 1)

            Next

        Catch
        End Try

        rtxtconsole.AppendText(DateTime.Now & "  ウィンドウの整列を行いました。" & vbCrLf)


    End Sub

    '■全てリロード
    Private Sub btnreloadall_Click(sender As Object, e As EventArgs) Handles btnreloadall.Click
        btnreload1.PerformClick()
        btnreload2.PerformClick()
        btnreload3.PerformClick()
        btnreload4.PerformClick()
        btnreload5.PerformClick()
        btnreload6.PerformClick()
        btnreload7.PerformClick()
        btnreload8.PerformClick()
        btnreload9.PerformClick()
        btnreload10.PerformClick()
        btnreload11.PerformClick()
        btnreload12.PerformClick()
        btnreload13.PerformClick()
        btnreload14.PerformClick()
        btnreload15.PerformClick()
        btnreload16.PerformClick()

    End Sub

    '■ブラウザを開く
    Private Sub btnopenbrowser_Click(sender As Object, e As EventArgs) Handles btnopenbrowser.Click

        For i = 0 To numbrowser_value.Value - 1
            Shell("""./nicobrowser.exe""", , False, 10000)

        Next

        If count_getpid = 0 Then
            '■1回目
            listbrowser.Items.Clear()
            listfirst.Items.Clear()

            '"nicobrowser"という名前のすべてのプロセスを取得
            Dim ps As System.Diagnostics.Process() =
            System.Diagnostics.Process.GetProcessesByName("nicobrowser")

            '配列から1つずつ取り出す
            Dim p As System.Diagnostics.Process
            For Each p In ps
                'IDとメインウィンドウのキャプションを出力する
                Console.WriteLine("{0}/{1}", p.Id, p.MainWindowTitle)
                listbrowser.Items.Add(p.Id)
                listfirst.Items.Add(p.Id)
            Next p
            add_pid()

            count_getpid = 1

        ElseIf count_getpid = 1 Then
            '■2回目以降
            listsecond.Items.Clear()
            listbrowser.Items.Clear()

            'ローカルコンピュータ上で実行されている"notepad"という名前の
            'すべてのプロセスを取得
            Dim ps As System.Diagnostics.Process() =
            System.Diagnostics.Process.GetProcessesByName("nicobrowser")

            '配列から1つずつ取り出す
            Dim p As System.Diagnostics.Process
            For Each p In ps
                'IDとメインウィンドウのキャプションを出力する
                Console.WriteLine("{0}/{1}", p.Id, p.MainWindowTitle)
                listsecond.Items.Add(p.Id)
            Next p

            'PIDの整理
            Dim second_no As Integer = listsecond.Items.Count
            Dim first_no As Integer = listfirst.Items.Count
            For ii = 0 To first_no - 1

                Dim x As Integer = -1
                x = listsecond.FindString(listfirst.Items(ii), x)
                If x <> -1 Then
                    '見つかったIDをlistbrowserに追加
                    '見つかったIDをlistsecondから削除
                    '残ったIDをlistbrowserに追加
                    listbrowser.Items.Add(listfirst.Items(ii))
                    listsecond.Items.Remove(listfirst.Items(ii))
                    'listtemp.SetSelected(x, True)
                    'MessageBox.Show(x + 1 & " 番目に見つかりました。")
                End If
            Next
            Dim lastcount As Integer = listsecond.Items.Count
            For iii = 0 To lastcount - 1
                listbrowser.Items.Add(listsecond.Items(iii))
            Next
            listfirst.Items.Clear()
            Dim finalcount As Integer = listbrowser.Items.Count
            For j = 0 To finalcount - 1
                listfirst.Items.Add(listbrowser.Items(j))
            Next

            add_pid()
        End If
        txtcount_browser.Text = listbrowser.Items.Count

    End Sub

    '■プロセスIDを取得する。
    Private count_getpid As Integer = 0
    Private Sub buttongetpid_Click(sender As Object, e As EventArgs) Handles buttongetpid.Click

        If count_getpid = 0 Then
            '■1回目
            listbrowser.Items.Clear()
            listfirst.Items.Clear()

            '"nicobrowser"という名前のすべてのプロセスを取得
            Dim ps As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName("nicobrowser")

            '配列から1つずつ取り出す
            Dim p As System.Diagnostics.Process
            For Each p In ps
                'IDとメインウィンドウのキャプションを出力する
                Console.WriteLine("{0}/{1}", p.Id, p.MainWindowTitle)
                listbrowser.Items.Add(p.Id)
                listfirst.Items.Add(p.Id)

            Next p

            add_pid()

            count_getpid = 1


        ElseIf count_getpid = 1 Then
            '■2回目以降
            listsecond.Items.Clear()
            listbrowser.Items.Clear()

            '"nicobrowser"という名前のすべてのプロセスを取得
            Dim ps As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName("nicobrowser")

            '配列から1つずつ取り出す
            Dim p As System.Diagnostics.Process

            For Each p In ps
                'IDとメインウィンドウのキャプションを出力する
                Console.WriteLine("{0}/{1}", p.Id, p.MainWindowTitle)
                listsecond.Items.Add(p.Id)

            Next p

            'PIDの整理
            Dim second_no As Integer = listsecond.Items.Count
            Dim first_no As Integer = listfirst.Items.Count

            For ii = 0 To first_no - 1
                Dim x As Integer = -1
                x = listsecond.FindString(listfirst.Items(ii), x)

                If x <> -1 Then
                    '見つかったIDをlistbrowserに追加
                    '見つかったIDをlistsecondから削除
                    '残ったIDをlistbrowserに追加
                    listbrowser.Items.Add(listfirst.Items(ii))
                    listsecond.Items.Remove(listfirst.Items(ii))
                    'listtemp.SetSelected(x, True)
                    'MessageBox.Show(x + 1 & " 番目に見つかりました。")
                End If

            Next

            Dim lastcount As Integer = listsecond.Items.Count

            For iii = 0 To lastcount - 1
                listbrowser.Items.Add(listsecond.Items(iii))

            Next


            listfirst.Items.Clear()


            Dim finalcount As Integer = listbrowser.Items.Count

            For j = 0 To finalcount - 1
                listfirst.Items.Add(listbrowser.Items(j))

            Next

            add_pid()


        End If

        txtcount_browser.Text = listbrowser.Items.Count


    End Sub

    Private Sub add_pid()

        btnweb1.Text = ""
        btnweb2.Text = ""
        btnweb3.Text = ""
        btnweb4.Text = ""
        btnweb5.Text = ""
        btnweb6.Text = ""
        btnweb7.Text = ""
        btnweb8.Text = ""
        btnweb9.Text = ""
        btnweb10.Text = ""
        btnweb11.Text = ""
        btnweb12.Text = ""
        btnweb13.Text = ""
        btnweb14.Text = ""
        btnweb15.Text = ""
        btnweb16.Text = ""

        Dim count_list As Integer = listbrowser.Items.Count

        Try
            btnweb1.Text = listbrowser.Items(0)
            btnweb2.Text = listbrowser.Items(1)
            btnweb3.Text = listbrowser.Items(2)
            btnweb4.Text = listbrowser.Items(3)
            btnweb5.Text = listbrowser.Items(4)
            btnweb6.Text = listbrowser.Items(5)
            btnweb7.Text = listbrowser.Items(6)
            btnweb8.Text = listbrowser.Items(7)
            btnweb9.Text = listbrowser.Items(8)
            btnweb10.Text = listbrowser.Items(9)
            btnweb11.Text = listbrowser.Items(10)
            btnweb12.Text = listbrowser.Items(11)
            btnweb13.Text = listbrowser.Items(12)
            btnweb14.Text = listbrowser.Items(13)
            btnweb15.Text = listbrowser.Items(14)
            btnweb16.Text = listbrowser.Items(15)

        Catch
        End Try


    End Sub

    '■マウスカーソルの位置を取得
    Private Sub timmouseposition_Tick(sender As Object, e As EventArgs) Handles timmouseposition.Tick

        lblposition.Text = "カーソルの位置: " & Cursor.Position.X & ", " & Cursor.Position.Y


    End Sub


    '■URLを埋め込み用URLに加工
    Private Sub dg1_KeyDown(sender As Object, e As KeyEventArgs) Handles dg1.KeyDown

        Dim x As Integer = dg1.CurrentCellAddress.X
        Dim y As Integer = dg1.CurrentCellAddress.Y
        Dim cel_SellectedCount As Integer = 0

        If (e.Modifiers And Keys.Control) = Keys.Control And e.KeyCode = Keys.V Then

            For Each c As DataGridViewCell In dg1.SelectedCells 'セルの選択数を取得
                cel_SellectedCount += 1

            Next

            If Not cel_SellectedCount = 1 Then '選択数が1以外ならキャンセル。
                MsgBox("セルを1つだけ選択して下さい。")
                Exit Sub

            End If


            dg1.Rows(y).Selected = True '次の行を選択する

            Dim aaa As Integer = dg1.SelectedRows(0).Index.ToString() '選択されている行を表示

            If dg1.RowCount - 1 <= aaa Then '最終行を選択している状態の場合、1行追加する。
                dg1.Rows.Add(1)
            End If


            '■クリップボードの内容を取得
            Dim clipText As String = Clipboard.GetText()
            Dim replaceClip As String = ""

            If 0 <= clipText.IndexOf("https://www.twitch.tv/") Then
                dg1(dg_co.Index, y).Value = "Twitch"
                replaceClip = clipText.Replace("https://www.twitch.tv/", "https://player.twitch.tv/?channel=")

                If dg1(dg_name.Index, y).Value = "" Or dg1(dg_name.Index, y).Value = "0" Then
                    Dim usernameTwitch As String = clipText.Replace("https://www.twitch.tv/", "")
                    dg1(dg_name.Index, y).Value = usernameTwitch

                End If


            ElseIf 0 <= clipText.IndexOf("https://www.youtube.com/watch?v=") Then
                dg1(dg_co.Index, y).Value = "Youtube"
                replaceClip = clipText.Replace("https://www.youtube.com/watch?v=", "https://www.youtube.com/embed/")


            ElseIf 0 <= clipText.IndexOf("https://mixer.com/") Then
                dg1(dg_co.Index, y).Value = "Mixer"
                replaceClip = clipText.Replace("https://mixer.com/", "https://mixer.com/embed/player/")

                If dg1(dg_name.Index, y).Value = "" Or dg1(dg_name.Index, y).Value = "0" Then
                    Dim usernameMixer As String = clipText.Replace("https://mixer.com/", "")

                    dg1(dg_name.Index, y).Value = usernameMixer

                End If

            End If

            dg1(x, y).Value = replaceClip
            Console.WriteLine(replaceClip)

            dg1.FirstDisplayedScrollingRowIndex = dg1.Rows.Count - 1 '先頭の行までスクロールする

        End If


    End Sub

    '■チェックボックス切替（2段階認証）
    Private Sub chk2factor_CheckedChanged(sender As Object, e As EventArgs) Handles chk2factor.CheckedChanged

        If chk2factor.Checked = True Then
            txtaddress.Enabled = False
            txtpass.Enabled = False
            txtuser_session.Enabled = True
            chklogin_auto.Checked = False
            chklogin_auto.Enabled = False

        Else
            txtaddress.Enabled = True
            txtpass.Enabled = True
            txtuser_session.Enabled = False
            chklogin_auto.Enabled = True

        End If


    End Sub


    Private Sub TabControl1_TabIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.Selected

        Me.Size = New Size(798, 646)


    End Sub




End Class









Namespace NicoVideoAPI
    Public Module NicoVideoAPI
        'ニコニコ動画にログインして会員ページに必要なクッキーを返します。
        Public Function Login(ByVal Mail As String, ByVal Password As String) As CookieContainer
            'データをPOSTする
            Dim content As String = "mail=" & Mail & "&password=" & Password
            Dim contentBytes As Byte() = Encoding.ASCII.GetBytes(content)
            Dim request As HttpWebRequest = HttpWebRequest.CreateHttp(
                "https://secure.nicovideo.jp/secure/login?site=niconico")
            request.CookieContainer = New CookieContainer
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = contentBytes.Length
            Using stream As Stream = request.GetRequestStream()
                stream.Write(contentBytes, 0, contentBytes.Length)
            End Using

            '応答を確認してcookieを取得する
            Using response As HttpWebResponse = request.GetResponse()
                Return request.CookieContainer
            End Using


        End Function

        'GETを送信してHTML、XML等を取得します。
        Public Function Read(ByRef cc As CookieContainer, ByVal URL As String) As String
            Dim request As HttpWebRequest = HttpWebRequest.CreateHttp(URL)
            request.CookieContainer = cc
            Using response As HttpWebResponse = request.GetResponse()
                Using reader As New StreamReader(response.GetResponseStream())
                    Return reader.ReadToEnd
                End Using
            End Using
        End Function
    End Module

End Namespace


