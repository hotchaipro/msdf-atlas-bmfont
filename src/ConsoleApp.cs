#nullable enable
using System;

namespace HotChai.CommandLine
{
    internal delegate int CommandDelegate(
        string[] parameters,
        Dictionary<string, string?> flags);

    internal delegate Task<int> AsyncCommandDelegate(
        string[] parameters,
        Dictionary<string, string?> flags);

    internal static class ConsoleResult
    {
        public const int Success = 0;
        public const int SyntaxError = -1;
    }

    internal sealed class ConsoleApp
    {
        private static readonly Dictionary<string, ConsoleCommand> Commands = new();

        public static void RegisterCommand(
            string name,
            CommandDelegate commandDelegate,
            string help)
        {
            Commands.Add(name, new ConsoleCommand(name, help, commandDelegate, null));
        }

        public static void RegisterAsyncCommand(
            string name,
            AsyncCommandDelegate asyncCommandDelegate,
            string help)
        {
            Commands.Add(name, new ConsoleCommand(name, help, null, asyncCommandDelegate));
        }

        private static int ShowHelp()
        {
            foreach (var command in Commands.Values)
            {
                ShowHelp(command);
            }

            return ConsoleResult.SyntaxError;
        }

        private static void ShowHelp(
            ConsoleCommand command)
        {
            Console.WriteLine($"{command.Name} {command.Help}");
        }

        public static async Task<int> Start(
            string[] args)
        {
            if (!ParseArgs(args, out string? commandName, out string[] parameters, out Dictionary<string, string?> flags))
            {
                return ShowHelp();
            }

            if (!Commands.TryGetValue(commandName!, out ConsoleCommand? command))
            {
                return ShowHelp();
            }

            int result;

            if (command.IsAsync)
            {
                result = await command.InvokeAsync(parameters, flags);
            }
            else
            {
                result = command.Invoke(parameters, flags);
            }

            if (result == ConsoleResult.SyntaxError)
            {
                ShowHelp(command);
            }

            return result;
        }

        private static bool ParseArgs(
            string[] args,
            out string? commandName,
            out string[] parameters,
            out Dictionary<string, string?> flags)
        {
            commandName = default;
            var parametersList = new List<string>();
            flags = new Dictionary<string, string?>();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (i == 0)
                {
                    commandName = arg;
                }
                else if ((arg.StartsWith('-')) || (arg.StartsWith('/')))
                {
                    var flagNameAndValue = arg.Substring(1);
                    var flagParts = flagNameAndValue.Split([':'], 2, StringSplitOptions.RemoveEmptyEntries);
                    if (flagParts.Length > 0)
                    {
                        flags.Add(flagParts[0], (flagParts.Length > 1) ? flagParts[1] : null);
                    }
                }
                else
                {
                    parametersList.Add(arg);
                }
            }

            parameters = parametersList.ToArray();

            return (commandName is not null);
        }

        private sealed class ConsoleCommand
        {
            private readonly CommandDelegate? _commandDelegate;
            private readonly AsyncCommandDelegate? _asyncCommandDelegate;

            public ConsoleCommand(
                string name,
                string help,
                CommandDelegate? commandDelegate,
                AsyncCommandDelegate? asyncCommandDelegate)
            {
                this.Name = name;
                this.Help = help;
                this._commandDelegate = commandDelegate;
                this._asyncCommandDelegate = asyncCommandDelegate;
            }

            public string Name
            {
                get;
            }

            public string Help
            {
                get;
            }

            public bool IsAsync
            {
                get
                {
                    return this._asyncCommandDelegate is not null;
                }
            }

            internal int Invoke(
                string[] parameters,
                Dictionary<string, string?> flags)
            {
                if (this._commandDelegate is null)
                {
                    return ConsoleResult.SyntaxError;
                }

                return this._commandDelegate(parameters, flags);
            }

            internal Task<int> InvokeAsync(
                string[] parameters,
                Dictionary<string, string?> flags)
            {
                if (this._asyncCommandDelegate is null)
                {
                    return Task.FromResult(ConsoleResult.SyntaxError);
                }

                return this._asyncCommandDelegate(parameters, flags);
            }
        }
    }
}
