Imports NUnit.Framework

Public Class TextTemplateTransformerTests

    <Test()>
    Public Sub Transforming_Template4WithInclude_ResultsInExpectedOutput()
        ' Arrange.
        Dim transformer = New TextTemplateTransformer()
        Dim expected = New My.Templates.Template1().TransformText()

        ' Act.
        Dim output = transformer.TransformFile("Templates\Template1.tt")

        ' Assert.
        Assert.AreEqual(expected, output)
    End Sub

End Class
