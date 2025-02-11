using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Abstractions.Models;
using StreamJsonRpc;

public class StdioClientTransport : ITransport
{
    // Process object
    private Process? _process = null;
    private StreamWriter? stdioIn = null;
    private StreamReader? stdioOut = null;

    // Store the command and arguments
    private string command;
    private string[] args;
    private Dictionary<string, string> env;
    private JsonRpc? jsonRpc = null;



    public StdioClientTransport(
        string command, string[]? args, Dictionary<string, string>? env)
	{
        // Store the paramters; we'll use them later
        this.command = command;
        this.args = args ?? new string[] { };
        this.env = env ?? GetDefaultEnvironment();
    }

    // Implement ITransport using Standard Input and Output
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Start the sub-process, and connect stdin/stdout
        _process = new Process();
        _process.StartInfo.FileName = command;

        // args array to single string
        _process.StartInfo.Arguments = string.Join(" ", args);
        // Add env to the process
        _process.StartInfo.EnvironmentVariables.Clear();
        foreach (var kvp in env)
        {
            _process.StartInfo.EnvironmentVariables[kvp.Key] = kvp.Value;
        }

        _process.StartInfo.RedirectStandardInput = true;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.CreateNoWindow = true;
        _process.Start();

        // implement jsonrpc over stdio
        jsonRpc = JsonRpc.Attach(_process.StandardInput.BaseStream, _process.StandardOutput.BaseStream);

        jsonRpc.StartListening();


        // Start reading from the process
        return Task.CompletedTask;
    }

    public async Task SendAsync(IRequest message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        _process?.Kill();
        _process?.Dispose();
        _process = null;
    }

    public event Action? OnClose;
    public event Action<Exception>? OnError;
    public event Action<IRequest>? OnMessage;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _process?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private Dictionary<string, string> GetDefaultEnvironment()
    {
        Dictionary<string, string> env = new Dictionary<string, string>();
        string[]? envNames = null;
        // If running on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            envNames = new string[] { "HOMEDRIVE", "HOMEPATH", "LOCALAPPDATA", "PATH", "PROCESSOR_ARCHITECTURE", "SYSTEMDRIVE", "SYSTEMROOT", "TEMP", "USERNAME", "USERPROFILE" };
        }
        else
        {
            envNames = new string[] { "HOME", "LOGNAME", "PATH", "SHELL", "TERM", "USER" };
        }

        foreach (string name in envNames!)
        {
            string? value = Environment.GetEnvironmentVariable(name);

            // No functions
            if (value != null && value.StartsWith("()") == false)
            {
                env[name]= value;
            }
        }

        return env;
    }
}
