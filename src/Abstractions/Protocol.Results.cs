using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;

// Result Base Type
public interface IResult
{
    [Description("Additional metadata for the result.")]
    Dictionary<string, object>? Meta { get; init; }
}
public interface IClientResult : IResult { }
public interface IServerResult : IResult { }


// export type ServerResult =
//   | EmptyResult
//   | InitializeResult
//   | CompleteResult
//   | GetPromptResult
//   | ListPromptsResult
//   | ListResourcesResult
//   | ReadResourceResult
//   | CallToolResult
//   | ListToolsResult;

// EmptyResult
public record EmptyResult : IClientResult, IServerResult
{
    [Description("Additional metadata associated with the result.")]
    public Dictionary<string, object>? Meta { get; init; }
}

// CreateMessageResult
public record CreateMessageResult : IResult, IClientResult
{
    [Description("Additional metadata for the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Required]
    [Description("The content of the created message.")]
    public IContent Content { get; init; } = new TextContent();

    [Required]
    [Description("The name of the model that generated the message.")]
    public string Model { get; init; } = string.Empty;

    [Required]
    [Description("The role of the sender of the message.")]
    public Role Role { get; init; }

    //  stopReason?: "endTurn" | "stopSequence" | "maxTokens" | string;
    [Description("The reason why sampling stopped, if known.")]
    public StopReason? StopReason { get; init; }
}

public enum StopReason
{
    EndTurn,
    StopSequence,
    MaxTokens
}

// ListRootsResult
public record ListRootsResult : IResult, IClientResult
{
    [Description("Additional metadata for the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Required]
    [Description("The list of roots available.")]
    public List<Root> Roots { get; init; } = new();
}


// ClientResult
public record ClientResult : IResult
{
    [Description("Additional metadata for the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Required]
    [Description("The actual result data.")]
    public object ResultData { get; init; } = new();
}

// CallToolResult
public record CallToolResult : IResult
{
    [Description("Additional metadata for the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Required]
    [Description("The content returned by the tool.")]
    public List<IContent> Content { get; init; } = new();

    [Description("Indicates whether the tool call resulted in an error.")]
    public bool IsError { get; init; } = false;
}

    // InitializeResult
    public record InitializeResult : IServerResult
    {
        [Description("Additional metadata associated with the result.")]
        public Dictionary<string, object>? Meta { get; init; }

        [Required]
        [Description("The server capabilities.")]
        public ServerCapabilities Capabilities { get; init; } = new ServerCapabilities();

        [Required]
        [Description("The protocol version used by the server.")]
        public string ProtocolVersion { get; init; } = string.Empty;

        [Required]
        [Description("Information about the server implementation.")]
        public Implementation ServerInfo { get; init; } = new Implementation();

        [Description("Instructions describing how to use the server.")]
        public string? Instructions { get; init; }
    }

    public record ServerCapabilities
    {
        [Description("Experimental capabilities supported by the server.")]
        public Dictionary<string, object>? Experimental { get; init; }

        [Description("Capabilities related to logging.")]
        public Dictionary<string, object>? Logging { get; init; }

        [Description("Capabilities related to resources.")]
        public ServerResources? Resources { get; init; }

        [Description("Capabilities related to prompts.")]
        public ServerPrompts? Prompts { get; init; }

        [Description("Capabilities related to tools.")]
        public ServerTools? Tools { get; init; }
    }

    public record ServerResources
    {
        [Description("Indicates if the server supports resource list change notifications.")]
        public bool? ListChanged { get; init; }

        [Description("Indicates if the server supports subscribing to resource updates.")]
        public bool? Subscribe { get; init; }
    }

    public record ServerPrompts
    {
        [Description("Indicates if the server supports prompt list change notifications.")]
        public bool? ListChanged { get; init; }
    }

    public record ServerTools
    {
        [Description("Indicates if the server supports tool list change notifications.")]
        public bool? ListChanged { get; init; }
    }

    // ListResourcesResult
    public record ListResourcesResult : IServerResult
    {
        [Description("Additional metadata associated with the result.")]
        public Dictionary<string, object>? Meta { get; init; }

        [Required]
        [Description("The list of resources available.")]
        public List<Resource> Resources { get; init; } = new();
    }

    public record Resource
    {
        [Required]
        [Description("The URI of the resource.")]
        public Uri Uri { get; init; } = new Uri("http://example.com");

        [Description("A human-readable name for the resource.")]
        public string? Name { get; init; }

        [Description("A description of the resource.")]
        public string? Description { get; init; }

        [Description("The MIME type of the resource.")]
        public string? MimeType { get; init; }
    }

// /**
//  * The server's response to a resources/read request from the client.
//  */
// export interface ReadResourceResult extends Result {
//   contents: (TextResourceContents | BlobResourceContents)[];
// }
public record ReadResourceResult : IResult, IServerResult
{
    [Description("Additional metadata associated with the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Required]
    [Description("The contents of the resource.")]
    public List<IReadResourceContent> Contents { get; init; } = new();
}
public interface IReadResourceContent : IResourceContent
{

}

    // ListPromptsResult
    public record ListPromptsResult : PaginatedResult, IServerResult
    {
        [Required]
        [Description("The list of prompts available.")]
        public List<Prompt> Prompts { get; init; } = new();
    }

// /**
//  * The server's response to a prompts/get request from the client.
//  */
// export interface GetPromptResult extends Result {
//   /**
//    * An optional description for the prompt.
//    */
//   description?: string;
//   messages: PromptMessage[];
// }
public record GetPromptResult : IResult
{
    [Description("Additional metadata associated with the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Description("An optional description for the prompt.")]
    public string? Description { get; init; }

    [Required]
    [Description("The messages associated with the prompt.")]
    public List<PromptMessage> Messages { get; init; } = new();
}


// /**
//  * The server's response to a prompts/list request from the client.
//  */
// export interface ListPromptsResult extends PaginatedResult {
//   prompts: Prompt[];
// }

public record PaginatedResult : IResult
{
    [Description("Additional metadata associated with the result.")]
    public Dictionary<string, object>? Meta { get; init; }

    [Description("The next cursor for pagination, if more results are available.")]
    public string? NextCursor { get; init; }
}
