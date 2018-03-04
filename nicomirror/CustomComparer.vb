'IComparerインターフェイスを実装した、並び替える方法を定義したクラス
Public Class CustomComparer
    Implements IComparer
    Private sOrder As Integer
    Private comparer As Comparer

    Public Sub New(ByVal order As SortOrder)
        Me.sOrder = IIf(order = SortOrder.Descending, -1, 1)
        Me.comparer = New Comparer(
            System.Globalization.CultureInfo.CurrentCulture)
    End Sub

    '並び替え方を定義する
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
         Implements System.Collections.IComparer.Compare
        Dim result As Integer = 0

        Dim rowx As DataGridViewRow = CType(x, DataGridViewRow)
        Dim rowy As DataGridViewRow = CType(y, DataGridViewRow)

        'はじめの列のセルの値を比較し、同じならば次の列を比較する
        For i As Integer = 0 To rowx.Cells.Count - 1
            result = Me.comparer.Compare(
                rowx.Cells(i).Value, rowy.Cells(i).Value)
            If result <> 0 Then
                Exit For
            End If
        Next i

        '結果を返す
        Return result * Me.sOrder
    End Function
End Class