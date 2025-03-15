using System.Linq;
using System.Text;

using EntityFrameworkCore.Generator.Parsing;

using Xunit;

namespace EntityFrameworkCore.Generator.Core.Tests.Parsing;

public class RegionParserTests
{
    [Fact]
    public void ParseRegions()
    {
        var parser = new RegionParser();

        var source = new StringBuilder();
        source.AppendLine(@"using System;");
        source.AppendLine(@"using System.Collections.Generic;");
        source.AppendLine(@"");
        source.AppendLine(@"namespace EntityFrameworkCore.Generator.Core.Tests");
        source.AppendLine(@"{");
        source.AppendLine(@"    public partial class User");
        source.AppendLine(@"    {");
        source.AppendLine(@"        #region Generated Properties");
        source.AppendLine(@"        public Guid Id { get; set; }");
        source.AppendLine(@"        #endregion");
        source.AppendLine(@"    }");
        source.AppendLine(@"}");


        var result = parser.ParseRegions(source.ToString());
        Assert.NotNull(result);
        Assert.Single(result);

        var first = result.Values.First();
        Assert.NotNull(first);
        Assert.Equal("Generated Properties", first.Name);

        var content = new StringBuilder();
        content.AppendLine(@"#region Generated Properties");
        content.AppendLine(@"        public Guid Id { get; set; }");
        content.AppendLine(@"        #endregion");

        Assert.Equal(content.ToString(), first.Content);

    }

    [Fact]
    public void ParseMultipleRegions()
    {
        var parser = new RegionParser();

        var source = new StringBuilder();
        source.AppendLine(@"using System;");
        source.AppendLine(@"using System.Collections.Generic;");
        source.AppendLine(@"");
        source.AppendLine(@"namespace EntityFrameworkCore.Generator.Core.Tests");
        source.AppendLine(@"{");
        source.AppendLine(@"    public partial class User");
        source.AppendLine(@"    {");
        source.AppendLine(@"        public User()");
        source.AppendLine(@"        {");
        source.AppendLine(@"            #region Generated Initializes");
        source.AppendLine(@"            Created = DateTimeOffset.UtcNow;");
        source.AppendLine(@"            Updated = DateTimeOffset.UtcNow;");
        source.AppendLine(@"            UserRoles = new HashSet<UserRole>();");
        source.AppendLine(@"            #endregion");
        source.AppendLine(@"        }");
        source.AppendLine(@"");
        source.AppendLine(@"        #region Generated Properties");
        source.AppendLine(@"        public Guid Id { get; set; }");
        source.AppendLine(@"        public string EmailAddress { get; set; }");
        source.AppendLine(@"        public DateTimeOffset Created { get; set; }");
        source.AppendLine(@"        public DateTimeOffset Updated { get; set; }");
        source.AppendLine(@"        #endregion");
        source.AppendLine(@"");
        source.AppendLine(@"        #region Generated Relationships");
        source.AppendLine(@"        public virtual ICollection<UserRole> UserRoles { get; set; }");
        source.AppendLine(@"        #endregion");
        source.AppendLine(@"    }");
        source.AppendLine(@"}");


        var result = parser.ParseRegions(source.ToString());
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        var first = result.Values.First();
        Assert.NotNull(first);
        Assert.Equal("Generated Initializes", first.Name);
    }

    [Fact]
    public void ParseNestedRegions()
    {
        var parser = new RegionParser();

        var source = new StringBuilder();
        source.AppendLine(@"using System;");
        source.AppendLine(@"using System.Collections.Generic;");
        source.AppendLine(@"");
        source.AppendLine(@"namespace EntityFrameworkCore.Generator.Core.Tests");
        source.AppendLine(@"{");
        source.AppendLine(@"    public partial class User");
        source.AppendLine(@"    {");
        source.AppendLine(@"        #region Generated Properties");
        source.AppendLine(@"        public Guid Id { get; set; }");
        source.AppendLine(@"");
        source.AppendLine(@"        #region Nested Properties");
        source.AppendLine(@"        public string EmailAddress { get; set; }");
        source.AppendLine(@"        #endregion");
        source.AppendLine(@"");
        source.AppendLine(@"        #endregion");
        source.AppendLine(@"    }");
        source.AppendLine(@"}");


        var result = parser.ParseRegions(source.ToString());
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var nested = result["Nested Properties"];
        Assert.NotNull(nested);
        Assert.Equal("Nested Properties", nested.Name);

        var nestedContent = new StringBuilder();
        nestedContent.AppendLine(@"#region Nested Properties");
        nestedContent.AppendLine(@"        public string EmailAddress { get; set; }");
        nestedContent.AppendLine(@"        #endregion");

        Assert.Equal(nestedContent.ToString(), nested.Content);

        var generated = result["Generated Properties"];
        Assert.NotNull(generated);
        Assert.Equal("Generated Properties", generated.Name);

        var generatedContent = new StringBuilder();
        generatedContent.AppendLine(@"#region Generated Properties");
        generatedContent.AppendLine(@"        public Guid Id { get; set; }");
        generatedContent.AppendLine(@"");
        generatedContent.AppendLine(@"        #region Nested Properties");
        generatedContent.AppendLine(@"        public string EmailAddress { get; set; }");
        generatedContent.AppendLine(@"        #endregion");
        generatedContent.AppendLine(@"");
        generatedContent.AppendLine(@"        #endregion");

        Assert.Equal(generatedContent.ToString(), generated.Content);

    }


}
