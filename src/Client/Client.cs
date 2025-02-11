using Abstractions.Models;
using StreamJsonRpc;

namespace MCP.Client
{
    public interface IModelContextProtocol
    {
        [JsonRpcMethod("tools/list")]
        string[] GetTools();
    }

    public class Client
    {
        public IModelContextProtocol? mcp = null;
        public ClientCapabilities Capabilities { get; set; }

        public Client(ClientCapabilities? capabilities = null)
        {
            Capabilities = capabilities ?? new ClientCapabilities();
        }

        public async Task Connect(JsonRpc rpc)
        {
            mcp = rpc.Attach<IModelContextProtocol>();
            rpc.StartListening();

        }

    }
}
