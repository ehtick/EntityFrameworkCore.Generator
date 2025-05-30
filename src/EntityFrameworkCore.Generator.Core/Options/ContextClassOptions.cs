using System.ComponentModel;

using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Generator.Options;

/// <summary>
/// EntityFramework <see cref="DbContext"/> generation options
/// </summary>
/// <seealso cref="ClassOptionsBase" />
public class ContextClassOptions : ClassOptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContextClassOptions"/> class.
    /// </summary>
    public ContextClassOptions(VariableDictionary variables, string? prefix)
        : base(variables, AppendPrefix(prefix, "Context"))
    {
        Namespace = "{Project.Namespace}.Data";
        Directory = @"{Project.Directory}\Data";

        Name = "{Database.Name}Context";
        BaseClass = "DbContext";
        PropertyNaming = ContextNaming.Plural;
    }

    /// <summary>
    /// Gets or sets the property naming strategy for entity data set property.
    /// </summary>
    /// <value>
    /// The property naming strategy for entity data set property.
    /// </value>
    [DefaultValue(ContextNaming.Plural)]
    public ContextNaming PropertyNaming { get; set; }
}
