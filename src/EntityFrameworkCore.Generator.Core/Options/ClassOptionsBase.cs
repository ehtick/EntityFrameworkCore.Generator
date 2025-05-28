using System;
using System.ComponentModel;

namespace EntityFrameworkCore.Generator.Options;

/// <summary>
/// Base class for Class generation
/// </summary>
public abstract class ClassOptionsBase : OptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassOptionsBase"/> class.
    /// </summary>
    protected ClassOptionsBase(VariableDictionary variables, string? prefix)
        : base(variables, prefix)
    {
        Namespace = "{Project.Namespace}";
        Directory = @"{Project.Directory}\";
        Document = false;
    }

    /// <summary>
    /// Gets or sets the class namespace.
    /// </summary>
    /// <value>
    /// The class namespace.
    /// </value>
    public string? Namespace
    {
        get => GetProperty();
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the output directory.  Default is the current working directory.
    /// </summary>
    /// <value>
    /// The output directory.
    /// </value>
    public string? Directory
    {
        get => GetProperty();
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to create xml documentation.
    /// </summary>
    /// <value>
    ///   <c>true</c> to create xml documentation; otherwise, <c>false</c>.
    /// </value>
    [DefaultValue(false)]
    public bool Document { get; set; }

    /// <summary>
    /// Gets or sets the class name template.
    /// </summary>
    /// <value>
    /// The class name template.
    /// </value>
    public string? Name
    {
        get => GetProperty();
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the base class to inherit from.
    /// </summary>
    /// <value>
    /// The base class.
    /// </value>
    public string? BaseClass
    {
        get => GetProperty();
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the attributes to add to the class
    /// </summary>
    /// <value>
    /// The attributes to add to the class
    /// </value>
    public string? Attributes
    {
        get => GetProperty();
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the file header.
    /// </summary>
    public string? Header
    {
        get => GetProperty();
        set => SetProperty(value);
    }
}
