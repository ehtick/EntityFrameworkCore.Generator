using System.ComponentModel;

namespace EntityFrameworkCore.Generator.Serialization;

/// <summary>
/// Base class for Class generation
/// </summary>
public abstract class ClassBase
{
    /// <summary>
    /// Gets or sets the class namespace.
    /// </summary>
    /// <value>
    /// The class namespace.
    /// </value>
    public string? Namespace { get; set; }

    /// <summary>
    /// Gets or sets the output directory.  Default is the current working directory.
    /// </summary>
    /// <value>
    /// The output directory.
    /// </value>
    public string? Directory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to create XML documentation.
    /// </summary>
    /// <value>
    ///   <c>true</c> to create XML documentation; otherwise, <c>false</c>.
    /// </value>
    [DefaultValue(false)]
    public bool Document { get; set; }

    /// <summary>
    /// Gets or sets the class name template.
    /// </summary>
    /// <value>
    /// The class name template.
    /// </value>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the base class to inherit from.
    /// </summary>
    /// <value>
    /// The base class.
    /// </value>
    public string? BaseClass { get; set; }

    /// <summary>
    /// Gets or sets the attributes to add to the class
    /// </summary>
    /// <value>
    /// The attributes to add to the class
    /// </value>
    public string? Attributes { get; set; }

    /// <summary>
    /// Gets or sets the file header.
    /// </summary>
    public string? Header { get; set; }
}
