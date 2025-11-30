Imports ALODWebUtility.TabNavigation

Public Structure SeniorMedicalReviewerTabVisibilityArgs
    Public ModuleId As Integer
    Public RefId As Integer
    Public Steps As TabItemList
    Public TabTitle As String
    Public WorkStatusIds As List(Of Integer)
End Structure