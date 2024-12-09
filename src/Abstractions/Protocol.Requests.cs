using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;
    // Base Request Type
    public interface IRequest
    {
        [Required]
        [Description("The method to identify the type of request.")]
        string Method { get; init; }
    }

    // InitializeRequest
    public record InitializeRequest : IRequest, IClientRequest
    {
        [Required]
        [Description("The method to initialize the request.")]
        public string Method { get; init; } = "initialize";

        [Required]
        public InitializeParams Params { get; init; } = new InitializeParams();
    }

    public record InitializeParams
    {
        [Required]
        public ClientCapabilities Capabilities { get; init; } = new ClientCapabilities();

        [Required]
        public Implementation ClientInfo { get; init; } = new Implementation();

        [Required]
        [Description("The protocol version supported by the client.")]
        public string ProtocolVersion { get; init; } = "1.0";
    }

    // PingRequest
    public record PingRequest : Request
    {
        [Required]
        [Description("The method to identify a ping request.")]
        public override string Method { get; init; } = "ping";
    }

    // ListRootsRequest
    public record ListRootsRequest : IRequest
    {
        [Required]
        [Description("The method to request a list of roots.")]
        public string Method { get; init; } = "roots/list";

        public ListRootsParams? Params { get; init; }
    }

    public record ListRootsParams
    {
        [Description("Optional metadata for the list roots request.")]
        public Dictionary<string, object>? Meta { get; init; }
    }

public abstract record Request : IRequest
{
    public abstract string Method { get; init; }
}
public abstract record PaginatedRequest : Request
{
    [Description("The pagination cursor for the request.")]
    public string? Cursor { get; init; }
}
// /* Resources */
// /**
//  * Sent from the client to request a list of resources the server has.
//  */
// export interface ListResourcesRequest extends PaginatedRequest {
//   method: "resources/list";
// }
public record ListResourcesRequest : PaginatedRequest, IRequest, IClientRequest
{
    [Required]
    [Description("The method to request a list of resources.")]
    public override string Method { get; init; } = "resources/list";
}


    // CallToolRequest
    public record CallToolRequest : IRequest
    {
        [Required]
        [Description("The method to invoke a tool.")]
        public string Method { get; init; } = "tools/call";

        [Required]
        public CallToolParams Params { get; init; } = new CallToolParams();
    }

    public record CallToolParams
    {
        [Required]
        [Description("The name of the tool to be invoked.")]
        public string Name { get; init; } = string.Empty;

        [Description("The arguments required for invoking the tool.")]
        public Dictionary<string, object>? Arguments { get; init; }
    }

// /**
//  * Sent from the client to the server, to read a specific resource URI.
//  */
// export interface ReadResourceRequest extends Request {
//   method: "resources/read";
//   params: {
//     /**
//      * The URI of the resource to read. The URI can use any protocol; it is up to the server how to interpret it.
//      *
//      * @format uri
//      */
//     uri: string;
//   };
// }
public record ReadResourceRequest : IRequest
{
    [Required]
    [Description("The method indicating a read resource request.")]
    public string Method { get; init; } = "resources/read";

    [Required]
    public Parameters Params { get; init; } = new Parameters();
    public record Parameters
    {
        [Required]
        [Description("The URI of the resource to read.")]
        public Uri Uri { get; init; } = new Uri("http://example.com");
    }
}


// /**
//  * Sent from the client to request a list of resource templates the server has.
//  */
// export interface ListResourceTemplatesRequest extends PaginatedRequest {
//   method: "resources/templates/list";
// }
public record ListResourceTemplatesRequest : IRequest
{
    [Required]
    [Description("The method to request a list of resource templates.")]
    public string Method { get; init; } = "resources/templates/list";
}

// /**
//  * The server's response to a resources/templates/list request from the client.
//  */
// export interface ListResourceTemplatesResult extends PaginatedResult {
//   resourceTemplates: ResourceTemplate[];
// }
public record ListResourceTemplatesResult : PaginatedResult, IResult
{
    [Required]
    [Description("The list of resource templates available.")]
    public List<ResourceTemplate> ResourceTemplates { get; init; } = new();
}

    // SubscribeRequest
    public record SubscribeRequest : IRequest
    {
        [Required]
        [Description("The method to subscribe to resource updates.")]
        public string Method { get; init; } = "resources/subscribe";

        [Required]
        public SubscribeParams Params { get; init; } = new SubscribeParams();
    }

    public record SubscribeParams
    {
        [Required]
        [Description("The URI of the resource to subscribe to.")]
        public Uri Uri { get; init; } = new Uri("http://example.com");
    }

    // UnsubscribeRequest
    public record UnsubscribeRequest : IRequest
    {
        [Required]
        [Description("The method to unsubscribe from resource updates.")]
        public string Method { get; init; } = "resources/unsubscribe";

        [Required]
        public UnsubscribeParams Params { get; init; } = new UnsubscribeParams();
    }

    public record UnsubscribeParams
    {
        [Required]
        [Description("The URI of the resource to unsubscribe from.")]
        public Uri Uri { get; init; } = new Uri("http://example.com");
    }

    // Base Client Request Interface
    public interface IClientRequest : IRequest
    {
    }

    // CompleteRequest
    public record CompleteRequest : IClientRequest
    {
        [Required]
        [Description("The method for a completion request.")]
        public string Method { get; init; } = "completion/complete";

        [Required]
        public CompleteParams Params { get; init; } = new CompleteParams();
    }

    public record CompleteParams
    {
        [Required]
        public CompletionArgument Argument { get; init; } = new CompletionArgument();

        [Required]
        public CompletionReference Ref { get; init; } = new PromptReference();
    }

    public record CompletionArgument
    {
        [Required]
        [Description("The name of the argument.")]
        public string Name { get; init; } = string.Empty;

        [Required]
        [Description("The value of the argument.")]
        public string Value { get; init; } = string.Empty;
    }

    public abstract record CompletionReference
    {
        [Required]
        [Description("The type of the reference.")]
        public abstract string Type { get; init; }
    }

    public record PromptReference : CompletionReference
    {
        [Required]
        [Description("The name of the prompt or template.")]
        public string Name { get; init; } = string.Empty;

        [Required]
        [Description("The type of the reference.")]
        public override string Type { get; init; } = "ref/prompt";
    }

    public record ResourceReference : CompletionReference
    {
        [Required]
        [Description("The URI or URI template of the resource.")]
        public string Uri { get; init; } = string.Empty;

        [Required]
        [Description("The type of the reference.")]
        public override string Type { get; init; } = "ref/resource";
    }

    public record CreateMessageRequest : IRequest
    {
        [Required]
        [Description("The method for creating a message.")]
        public string Method { get; init; } = "messages/create";

        [Required]
        public CreateMessageParams Params { get; init; } = new CreateMessageParams();
    }

    // messages: SamplingMessage[];
    // /**
    //  * The server's preferences for which model to select. The client MAY ignore these preferences.
    //  */
    // modelPreferences?: ModelPreferences;
    // /**
    //  * An optional system prompt the server wants to use for sampling. The client MAY modify or omit this prompt.
    //  */
    // systemPrompt?: string;
    // /**
    //  * A request to include context from one or more MCP servers (including the caller), to be attached to the prompt. The client MAY ignore this request.
    //  */
    // includeContext?: "none" | "thisServer" | "allServers";
    // /**
    //  * @TJS-type number
    //  */
    // temperature?: number;
    // /**
    //  * The maximum number of tokens to sample, as requested by the server. The client MAY choose to sample fewer tokens than requested.
    //  */
    // maxTokens: number;
    // stopSequences?: string[];
    // /**
    //  * Optional metadata to pass through to the LLM provider. The format of this metadata is provider-specific.
    //  */
    // metadata?: object;
    public record CreateMessageParams
    {
        public List<SamplingMessage> Messages { get; init; } = new();
        public ModelPreferences? ModelPreferences { get; init; }
        public string? SystemPrompt { get; init; }
        public IncludeContext? IncludeContext { get; init; }
        public double? Temperature { get; init; }
        public int MaxTokens { get; init; }
        public List<string>? StopSequences { get; init; }
        public Dictionary<string, object>? Metadata { get; init; }
    }


// /**
//  * The server's preferences for model selection, requested of the client during sampling.
//  *
//  * Because LLMs can vary along multiple dimensions, choosing the "best" model is
//  * rarely straightforward.  Different models excel in different areas—some are
//  * faster but less capable, others are more capable but more expensive, and so
//  * on. This interface allows servers to express their priorities across multiple
//  * dimensions to help clients make an appropriate selection for their use case.
//  *
//  * These preferences are always advisory. The client MAY ignore them. It is also
//  * up to the client to decide how to interpret these preferences and how to
//  * balance them against other considerations.
//  */
// export interface ModelPreferences {
//   /**
//    * Optional hints to use for model selection.
//    *
//    * If multiple hints are specified, the client MUST evaluate them in order
//    * (such that the first match is taken).
//    *
//    * The client SHOULD prioritize these hints over the numeric priorities, but
//    * MAY still use the priorities to select from ambiguous matches.
//    */
//   hints?: ModelHint[];

//   /**
//    * How much to prioritize cost when selecting a model. A value of 0 means cost
//    * is not important, while a value of 1 means cost is the most important
//    * factor.
//    *
//    * @TJS-type number
//    * @minimum 0
//    * @maximum 1
//    */
//   costPriority?: number;

//   /**
//    * How much to prioritize sampling speed (latency) when selecting a model. A
//    * value of 0 means speed is not important, while a value of 1 means speed is
//    * the most important factor.
//    *
//    * @TJS-type number
//    * @minimum 0
//    * @maximum 1
//    */
//   speedPriority?: number;

//   /**
//    * How much to prioritize intelligence and capabilities when selecting a
//    * model. A value of 0 means intelligence is not important, while a value of 1
//    * means intelligence is the most important factor.
//    *
//    * @TJS-type number
//    * @minimum 0
//    * @maximum 1
//    */
//   intelligencePriority?: number;
// }
public record ModelPreferences
{
    [Description("Optional hints to use for model selection.")]
    public List<ModelHint>? Hints { get; init; }
    [Description("How much to prioritize cost when selecting a model.")]
    [Range(0, 1)]
    public double? CostPriority { get; init; }
    [Description("How much to prioritize sampling speed (latency) when selecting a model.")]
    [Range(0, 1)]
    public double? SpeedPriority { get; init; }
    [Description("How much to prioritize intelligence and capabilities when selecting a model.")]
    [Range(0, 1)]
    public double? IntelligencePriority { get; init; }
}

// /**
//  * Hints to use for model selection.
//  *
//  * Keys not declared here are currently left unspecified by the spec and are up
//  * to the client to interpret.
//  */
// export interface ModelHint {
//   /**
//    * A hint for a model name.
//    *
//    * The client SHOULD treat this as a substring of a model name; for example:
//    *  - `claude-3-5-sonnet` should match `claude-3-5-sonnet-20241022`
//    *  - `sonnet` should match `claude-3-5-sonnet-20241022`, `claude-3-sonnet-20240229`, etc.
//    *  - `claude` should match any Claude model
//    *
//    * The client MAY also map the string to a different provider's model name or a different model family, as long as it fills a similar niche; for example:
//    *  - `gemini-1.5-flash` could match `claude-3-haiku-20240307`
//    */
//   name?: string;
// }
public record ModelHint
{
    public string? Name { get; init; }
}

    public enum IncludeContext
    {
        None,
        ThisServer,
        AllServers
    }

//     export interface SetLevelRequest extends Request {
//   method: "logging/setLevel";
//   params: {
//     /**
//      * The level of logging that the client wants to receive from the server. The server should send all logs at this level and higher (i.e., more severe) to the client as notifications/logging/message.
//      */
//     level: LoggingLevel;
//   };
// }
    public record SetLevelRequest : IRequest
    {
        [Required]
        [Description("The method for setting the logging level.")]
        public string Method { get; init; } = "logging/setLevel";

        [Required]
        public SetLevelParams Params { get; init; } = new SetLevelParams();
    }

    public record SetLevelParams
    {
        [Required]
        [Description("The level of logging that the client wants to receive from the server.")]
        public LoggingLevel Level { get; init; }
    }

public record GetPromptRequest : IRequest
{
    [Required]
    [Description("The method for getting a prompt.")]
    public string Method { get; init; } = "prompts/get";

    [Required]
    public GetPromptParams Params { get; init; } = new GetPromptParams();
}
public record GetPromptParams
{
    [Required]
    [Description("The name of the prompt or prompt template.")]
    public string Name { get; init; } = string.Empty;

    [Description("Arguments to use for templating the prompt.")]
    public Dictionary<string, string>? Arguments { get; init; }
}

public record ListPromptsRequest : IRequest
{
    [Required]
    [Description("The method for listing prompts.")]
    public string Method { get; init; } = "prompts/list";
}