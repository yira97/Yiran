namespace Blog.Domain.Enums;

public class CodeSnippetLanguage
{
    public const string CSharp = "cs";
    public const string CSharpRazor ="cshtml";
    public const string Swift ="swift";
    public const string Html ="html";
    public const string Css ="css";
    public const string JavaScript ="js";
    public const string Python ="py";
    public const string Bash ="bash";
    public const string Sql = "sql";
    public const string ProtocolBuffers = "protobuf";
    public const string Yaml = "yaml";
    
    public static string GetLanguageName(string language)
    {
        return language switch
        {
            CSharp => "C#",
            CSharpRazor => "C# Razor",
            Swift => "Swift",
            Html => "HTML",
            Css => "CSS",
            JavaScript => "JavaScript",
            Python => "Python",
            Bash => "Bash",
            Sql => "SQL",
            ProtocolBuffers => "Protobuf",
            Yaml => "YAML",
            _ => language
        };
    }
    
    public static string[] All => new[]
    {
        CSharp,
        CSharpRazor,
        Swift,
        Html,
        Css,
        JavaScript,
        Python,
        Bash,
        Sql,
        ProtocolBuffers,
        Yaml
    };
    
    public static string GetPrismjsClassName(string language)
    {
        return language switch
        {
            CSharp => "language-csharp",
            CSharpRazor => "language-cshtml",
            Swift => "language-swift",
            Html => "language-html",
            Css => "language-css",
            JavaScript => "language-javascript",
            Python => "language-python",
            Bash => "language-bash",
            Sql => "language-sql",
            ProtocolBuffers => "language-protobuf",
            Yaml => "language-yaml",
            _ => language
        };
    }
}