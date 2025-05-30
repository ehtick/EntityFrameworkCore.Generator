using System.Linq;

using EntityFrameworkCore.Generator.Extensions;
using EntityFrameworkCore.Generator.Metadata.Generation;
using EntityFrameworkCore.Generator.Options;

namespace EntityFrameworkCore.Generator.Templates;

public class QueryExtensionTemplate : CodeTemplateBase
{
    private readonly Entity _entity;

    public QueryExtensionTemplate(Entity entity, GeneratorOptions options) : base(options)
    {
        _entity = entity;
    }

    public override string WriteCode()
    {
        CodeBuilder.Clear();

        if (Options.Data.Query.Header.HasValue())
            CodeBuilder.AppendLine(Options.Data.Query.Header).AppendLine();

        CodeBuilder.AppendLine("using System;");
        CodeBuilder.AppendLine("using System.Collections.Generic;");
        CodeBuilder.AppendLine("using System.Linq;");
        CodeBuilder.AppendLine("using System.Threading;");
        CodeBuilder.AppendLine("using System.Threading.Tasks;");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
        CodeBuilder.AppendLine();

        var extensionNamespace = Options.Data.Query.Namespace;

        CodeBuilder.Append($"namespace {extensionNamespace}");

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
        var entityClass = _entity.EntityClass.ToSafeName();
        string safeName = _entity.EntityNamespace + "." + entityClass;

        if (Options.Data.Query.Document)
        {
            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine($"/// Query extensions for entity <see cref=\"{safeName}\" />.");
            CodeBuilder.AppendLine("/// </summary>");
        }

        CodeBuilder.AppendLine($"public static partial class {entityClass}Extensions");
        CodeBuilder.AppendLine("{");

        using (CodeBuilder.Indent())
        {
            GenerateMethods();
        }

        CodeBuilder.AppendLine("}");

    }

    private void GenerateMethods()
    {
        CodeBuilder.AppendLine("#region Generated Extensions");
        foreach (var method in _entity.Methods.OrderBy(m => m.NameSuffix))
        {
            if (method.IsKey)
            {
                GenerateKeyMethod(method);
                GenerateKeyMethod(method, true);
            }
            else if (method.IsUnique)
            {
                GenerateUniqueMethod(method);
                GenerateUniqueMethod(method, true);
            }
            else
            {
                GenerateMethod(method);
            }
        }
        CodeBuilder.AppendLine("#endregion");
        CodeBuilder.AppendLine();

    }

    private void GenerateMethod(Method method)
    {
        var safeName = _entity.EntityNamespace + "." + _entity.EntityClass.ToSafeName();
        var prefix = Options.Data.Query.IndexPrefix;
        var suffix = method.NameSuffix;

        if (Options.Data.Query.Document)
        {
            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine("/// Filters a sequence of values based on a predicate.");
            CodeBuilder.AppendLine("/// </summary>");
            CodeBuilder.AppendLine("/// <param name=\"queryable\">An <see cref=\"T:System.Linq.IQueryable`1\" /> to filter.</param>");
            AppendDocumentation(method);
            CodeBuilder.AppendLine("/// <returns>An <see cref=\"T: System.Linq.IQueryable`1\" /> that contains elements from the input sequence that satisfy the condition specified.</returns>");
        }

        CodeBuilder.Append($"public static System.Linq.IQueryable<{safeName}> {prefix}{suffix}(this System.Linq.IQueryable<{safeName}> queryable, ");
        AppendParameters(method);
        CodeBuilder.AppendLine(")");
        CodeBuilder.AppendLine("{");

        using (CodeBuilder.Indent())
        {
            CodeBuilder.AppendLine("if (queryable is null)");
            using (CodeBuilder.Indent())
                CodeBuilder.AppendLine("throw new ArgumentNullException(nameof(queryable));");
            CodeBuilder.AppendLine();

            CodeBuilder.Append("return queryable.Where(");
            AppendLamba(method);
            CodeBuilder.AppendLine(");");
        }

        CodeBuilder.AppendLine("}");
        CodeBuilder.AppendLine();
    }

    private void GenerateUniqueMethod(Method method, bool async = false)
    {
        var safeName = _entity.EntityNamespace + "." + _entity.EntityClass.ToSafeName();
        var uniquePrefix = Options.Data.Query.UniquePrefix;
        var suffix = method.NameSuffix;

        var asyncSuffix = async ? "Async" : string.Empty;
        var asyncPrefix = async ? "async " : string.Empty;
        var awaitPrefix = async ? "await " : string.Empty;
        var nullableSuffix = Options.Project.Nullable ? "?" : "";
        var returnType = async ? $"System.Threading.Tasks.Task<{safeName}{nullableSuffix}>" : safeName + nullableSuffix;

        if (Options.Data.Query.Document)
        {
            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine($"/// Gets an instance of <see cref=\"T:{safeName}\"/> by using a unique index.");
            CodeBuilder.AppendLine("/// </summary>");
            CodeBuilder.AppendLine("/// <param name=\"queryable\">An <see cref=\"T:System.Linq.IQueryable`1\" /> to filter.</param>");
            AppendDocumentation(method);

            if (async)
                CodeBuilder.AppendLine("/// <param name=\"cancellationToken\">A <see cref=\"System.Threading.CancellationToken\" /> to observe while waiting for the task to complete.</param>");

            CodeBuilder.AppendLine($"/// <returns>An instance of <see cref=\"T:{safeName}\"/> or null if not found.</returns>");
        }

        CodeBuilder.Append($"public static {asyncPrefix}{returnType} {uniquePrefix}{suffix}{asyncSuffix}(this System.Linq.IQueryable<{safeName}> queryable, ");
        AppendParameters(method);

        if (async)
            CodeBuilder.Append(", System.Threading.CancellationToken cancellationToken = default");

        CodeBuilder.AppendLine(")");
        CodeBuilder.AppendLine("{");

        using (CodeBuilder.Indent())
        {
            CodeBuilder.AppendLine("if (queryable is null)");
            using (CodeBuilder.Indent())
                CodeBuilder.AppendLine("throw new ArgumentNullException(nameof(queryable));");
            CodeBuilder.AppendLine();


            CodeBuilder.Append($"return {awaitPrefix}queryable.FirstOrDefault{asyncSuffix}(");

            AppendLamba(method);

            if (async)
                CodeBuilder.Append(", cancellationToken");

            CodeBuilder.AppendLine(");");
        }

        CodeBuilder.AppendLine("}");
        CodeBuilder.AppendLine();
    }

    private void GenerateKeyMethod(Method method, bool async = false)
    {
        var safeName = _entity.EntityNamespace + "." + _entity.EntityClass.ToSafeName();
        var uniquePrefix = Options.Data.Query.UniquePrefix;

        var asyncSuffix = async ? "Async" : string.Empty;
        var asyncPrefix = async ? "async " : string.Empty;
        var awaitPrefix = async ? "await " : string.Empty;
        var nullableSuffix = Options.Project.Nullable ? "?" : "";
        var returnType = async ? $"System.Threading.Tasks.ValueTask<{safeName}{nullableSuffix}>" : safeName + nullableSuffix;

        if (Options.Data.Query.Document)
        {
            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine("/// Gets an instance by the primary key.");
            CodeBuilder.AppendLine("/// </summary>");
            CodeBuilder.AppendLine("/// <param name=\"queryable\">An <see cref=\"T:System.Linq.IQueryable`1\" /> to filter.</param>");
            AppendDocumentation(method);

            if (async)
                CodeBuilder.AppendLine("/// <param name=\"cancellationToken\">A <see cref=\"System.Threading.CancellationToken\" /> to observe while waiting for the task to complete.</param>");

            CodeBuilder.AppendLine($"/// <returns>An instance of <see cref=\"T:{safeName}\"/> or null if not found.</returns>");
        }

        CodeBuilder.Append($"public static {asyncPrefix}{returnType} {uniquePrefix}Key{asyncSuffix}(this System.Linq.IQueryable<{safeName}> queryable, ");
        AppendParameters(method);

        if (async)
            CodeBuilder.Append(", System.Threading.CancellationToken cancellationToken = default");

        CodeBuilder.AppendLine(")");
        CodeBuilder.AppendLine("{");

        using (CodeBuilder.Indent())
        {
            CodeBuilder.AppendLine("if (queryable is null)");
            using (CodeBuilder.Indent())
                CodeBuilder.AppendLine("throw new ArgumentNullException(nameof(queryable));");
            CodeBuilder.AppendLine();

            CodeBuilder.AppendLine($"if (queryable is DbSet<{safeName}> dbSet)");
            using (CodeBuilder.Indent())
            {
                CodeBuilder.Append($"return {awaitPrefix}dbSet.Find{asyncSuffix}(");

                if (async)
                    CodeBuilder.Append("new object[] { ");

                AppendNames(method);

                if (async)
                    CodeBuilder.Append(" }, cancellationToken");

                CodeBuilder.AppendLine(");");
            }

            CodeBuilder.AppendLine("");
            CodeBuilder.Append($"return {awaitPrefix}queryable.FirstOrDefault{asyncSuffix}(");

            AppendLamba(method);

            if (async)
                CodeBuilder.Append(", cancellationToken");

            CodeBuilder.AppendLine(");");
        }
        CodeBuilder.AppendLine("}");
        CodeBuilder.AppendLine();
    }


    private void AppendDocumentation(Method method)
    {
        foreach (var property in method.Properties)
        {
            string paramName = property.PropertyName
                .ToCamelCase()
                .ToSafeName();

            CodeBuilder.AppendLine($"/// <param name=\"{paramName}\">The value to filter by.</param>");
        }
    }

    private void AppendParameters(Method method)
    {
        bool wrote = false;

        foreach (var property in method.Properties)
        {
            if (wrote)
                CodeBuilder.Append(", ");

            var paramName = property.PropertyName
                .ToCamelCase()
                .ToSafeName();

            var paramType = property.SystemType
                .ToNullableType(property.IsNullable == true);

            CodeBuilder.Append($"{paramType} {paramName}");

            wrote = true;
        }
    }

    private void AppendNames(Method method)
    {
        bool wrote = false;
        foreach (var property in method.Properties)
        {
            if (wrote)
                CodeBuilder.Append(", ");

            string paramName = property.PropertyName
                .ToCamelCase()
                .ToSafeName();

            CodeBuilder.Append(paramName);
            wrote = true;
        }
    }

    private void AppendLamba(Method method)
    {
        bool wrote = false;
        bool indented = false;

        foreach (var property in method.Properties)
        {
            string paramName = property.PropertyName
                .ToCamelCase()
                .ToSafeName();

            if (!wrote)
            {
                CodeBuilder.Append("q => ");
            }
            else
            {
                CodeBuilder.AppendLine();
                CodeBuilder.IncrementIndent();
                CodeBuilder.Append("&& ");

                indented = true;
            }

            if (property.IsNullable == true)
                CodeBuilder.Append($"(q.{property.PropertyName} == {paramName} || ({paramName} == null && q.{property.PropertyName} == null))");
            else
                CodeBuilder.Append($"q.{property.PropertyName} == {paramName}");

            wrote = true;
        }

        if (indented)
            CodeBuilder.DecrementIndent();
    }
}
