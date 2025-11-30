Option Strict On

Imports System.ComponentModel

Namespace Web.Controls

    Partial Class Controls_ModalDialog
        Inherits System.Web.UI.UserControl

        Private _contentTemplate As ITemplate = Nothing
        Private _showDialog As Boolean = False

#Region " Properties "
        <PersistenceMode(PersistenceMode.InnerDefaultProperty), _
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
         TemplateContainer(GetType(ControlContainer)), _
         TemplateInstance(TemplateInstance.Single)> _
        Public Property ContentTemplate() As ITemplate
            Get
                Return _contentTemplate
            End Get
            Set(ByVal value As ITemplate)
                _contentTemplate = value
            End Set
        End Property

        Public Property BehaviorID() As String
            Get
                Return ModalPopupExtender1.BehaviorID
            End Get
            Set(ByVal value As String)
                ModalPopupExtender1.BehaviorID = value
            End Set
        End Property

        Public ReadOnly Property PanelClientID() As String
            Get
                Return divContent.ClientID
            End Get
        End Property

        Public Property Title() As String
            Get
                Return ModalDialogTitle.InnerText
            End Get
            Set(ByVal value As String)
                ModalDialogTitle.InnerText = value
            End Set
        End Property

        Public Property Height() As String
            Get
                Return pnlContent.Style("height")
            End Get
            Set(ByVal value As String)
                pnlContent.Style("height") = value
            End Set
        End Property

        Public Property Width() As String
            Get
                Return pnlContent.Style("width")
            End Get
            Set(ByVal value As String)
                pnlContent.Style("width") = value
            End Set
        End Property
#End Region

#Region " ControlContainer Class "
        Public Class ControlContainer
            Inherits Control
            Implements INamingContainer

            Friend Sub New()
            End Sub
        End Class
#End Region

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If _contentTemplate IsNot Nothing Then
                Dim container As New ControlContainer()
                _contentTemplate.InstantiateIn(container)
                phContent.Controls.Add(container)
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                imgClose.Attributes.Add("onclick", "$find('" & Me.BehaviorID & "').hide();")

                'JS.Text = "<script type='text/javascript'>" & _
                ' "var dialog=document.getElementById('" & pnlContent.ClientID & "');" & _
                ' "var body=document.documentElement;" & _
                ' "if (dialog.scrollHeight>body.clientHeight) dialog.style.height=body.clientHeight-30;" & _
                ' "if (dialog.scrollWidth>body.clientWidth) dialog.style.width=body.clientWidth-30;" & _
                ' "</script>"
            End If
        End Sub

        Public Sub Hide()
            ModalPopupExtender1.Hide()
        End Sub

        Public Sub Show()
            ModalPopupExtender1.Show()
        End Sub

    End Class

End Namespace