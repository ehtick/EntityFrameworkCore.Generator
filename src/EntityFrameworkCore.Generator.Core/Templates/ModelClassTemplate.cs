using EntityFrameworkCore.Generator.Extensions;
using EntityFrameworkCore.Generator.Metadata.Generation;
using EntityFrameworkCore.Generator.Options;

namespace EntityFrameworkCore.Generator.Templates;

public class ModelClassTemplate : CodeTemplateBase
{
    private readonly Model _model;

    public ModelClassTemplate(Model model, GeneratorOptions options) : base(options)
    {
        _model = model;
    }

    public override string WriteCode()
    {
        CodeBuilder.Clear();

        if (_model.ModelHeader.HasValue())
            CodeBuilder.AppendLine(_model.ModelHeader).AppendLine();

        CodeBuilder.AppendLine("using System;");
        CodeBuilder.AppendLine("using System.Collections.Generic;");
        CodeBuilder.AppendLine();

        CodeBuilder.Append($"namespace {_model.ModelNamespace}");

        if (Options.Project.FileScopedNamespace)
        {
            CodeBuilder.AppendLine(";");
            CodeBuilder.AppendLine();
            GenerateClass();
        }
        else
        {
            CodeBuilder.AppendLine();
            CodeBuilder.AppendLine("{");

            using (CodeBuilder.Indent())
            {
                GenerateClass();
            }

            CodeBuilder.AppendLine("}");
        }

        return CodeBuilder.ToString();
    }

    private void GenerateClass()
    {
        var modelClass = _model.ModelClass.ToSafeName();


        if (ShouldDocument())
        {
            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine("/// View Model class");
            CodeBuilder.AppendLine("/// </summary>");
        }
        if (_model.ModelAttributes.HasValue())
        {
            CodeBuilder.AppendLine(_model.ModelAttributes);
        }
        CodeBuilder.AppendLine($"public partial class {modelClass}");

        if (_model.ModelBaseClass.HasValue())
        {
            var modelBase = _model.ModelBaseClass.ToSafeName();
            using (CodeBuilder.Indent())
                CodeBuilder.AppendLine($": {modelBase}");
        }

        CodeBuilder.AppendLine("{");

        using (CodeBuilder.Indent())
        {
            GenerateProperties();
        }

        CodeBuilder.AppendLine("}");

    }


    private void GenerateProperties()
    {
        CodeBuilder.AppendLine("#region Generated Properties");
        foreach (var property in _model.Properties)
        {
            var propertyType = property.SystemType.ToType();
            var propertyName = property.PropertyName.ToSafeName();

            if (ShouldDocument())
            {
                CodeBuilder.AppendLine("/// <summary>");
                CodeBuilder.AppendLine($"/// Gets or sets the property value for '{property.PropertyName}'.");
                CodeBuilder.AppendLine("/// </summary>");
                CodeBuilder.AppendLine("/// <value>");
                CodeBuilder.AppendLine($"/// The property value for '{property.PropertyName}'.");
                CodeBuilder.AppendLine("/// </value>");
            }

            if (property.IsNullable == true && (property.SystemType.IsValueType || Options.Project.Nullable))
                CodeBuilder.AppendLine($"public {propertyType}? {propertyName} {{ get; set; }}");
            else if (Options.Project.Nullable && !property.SystemType.IsValueType)
                CodeBuilder.AppendLine($"public {propertyType} {propertyName} {{ get; set; }} = null!;");
            else
                CodeBuilder.AppendLine($"public {propertyType} {propertyName} {{ get; set; }}");

            CodeBuilder.AppendLine();
        }
        CodeBuilder.AppendLine("#endregion");
        CodeBuilder.AppendLine();
    }


    private bool ShouldDocument()
    {
        if (_model.ModelType == ModelType.Create)
            return Options.Model.Create.Document;

        if (_model.ModelType == ModelType.Update)
            return Options.Model.Update.Document;

        return Options.Model.Read.Document;
    }
}
