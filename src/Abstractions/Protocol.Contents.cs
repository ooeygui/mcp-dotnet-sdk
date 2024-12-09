
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;

public interface IContent
{
}

    public interface IPromptContent : IContent
    {
    }
public interface ISamplingContent : IContent { }

public record TextContent : IContent, ISamplingContent, IPromptContent
{
    [Required]
    [Description("The type of the content, always 'text'.")]
    public string Type { get; init; } = "text";

    [Required]
    [Description("The text content.")]
    public string Text { get; init; } = string.Empty;

    public Annotations? Annotations { get; init; }
}

public record ImageContent : IContent, ISamplingContent, IPromptContent
{
    [Required]
    [Description("The type of the content, always 'image'.")]
    public string Type { get; init; } = "image";

    [Required]
    [Description("The base64-encoded image data.")]
    public string Data { get; init; } = string.Empty;

    [Description("The MIME type of the image.")]
    public string MimeType { get; init; } = string.Empty;

    public Annotations? Annotations { get; init; }
}

public record EmbeddedResource : IContent, IAnnotated, IPromptContent
{
    [Required]
    [Description("The type of the content, always 'resource'.")]
    public string Type { get; init; } = "resource";

    [Required]
    public IResourceContent Resource { get; init; } = new TextResourceContents();

    public Annotations? Annotations { get; set; }
}

public interface IResourceContent
{
    [Description("The MIME type of the resource.")]
    string MimeType { get; init; }

    [Description("The URI of the resource.")]
    Uri Uri { get; init; }
}

public record TextResourceContents : IResourceContent, IReadResourceContent
{
    [Description("The MIME type of the resource.")]
    public string MimeType { get; init; } = string.Empty;

    [Required]
    [Description("The text content of the resource.")]
    public string Text { get; init; } = string.Empty;

    [Required]
    [Description("The URI of the resource.")]
    public Uri Uri { get; init; } = new Uri("http://example.com");
}

// BlobResourceContents
public record BlobResourceContents : IResourceContent, IReadResourceContent
{
    [Required]
    [Description("A base64-encoded string representing the binary data of the item.")]
    public string Blob { get; init; } = string.Empty;

    [Description("The MIME type of this resource, if known.")]
    public string MimeType { get; init; } = string.Empty;

    [Required]
    [Description("The URI of this resource.")]
    public Uri Uri { get; init; } = new Uri("http://example.com");
}
