using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

struct TuringMachineCommand
{
    public string CurrentSymbol;
    public string CurrentState;    
    public string NewSymbol;
    public char Action;
    public string NewState;
}

class Program
{
    static List<TuringMachineCommand> ReadCommandsFromFile(string filename)
    {
        List<TuringMachineCommand> commands = new List<TuringMachineCommand>();
        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Match match = Regex.Match(line, @"^(?<currentSymbol>a\d+)(?<currentState>q\d+)\t(?<newSymbol>a\d+)(?<action>[REL])(?<newState>q\d+)$");
                if (match.Success)
                {
                    TuringMachineCommand command = new TuringMachineCommand
                    {
                        CurrentSymbol = match.Groups["currentSymbol"].Value,
                        CurrentState = match.Groups["currentState"].Value,
                        Action = match.Groups["action"].Value[0],
                        NewSymbol = match.Groups["newSymbol"].Value,
                        NewState = match.Groups["newState"].Value
                    };
                    commands.Add(command);
                }
            }
        }
        return commands;
    }
    static int GetSymbolIndex(string symbol)
    {
        return int.Parse(symbol.Substring(1));
    }

    static int GetStateIndex(string state)
    {
        return int.Parse(state.Substring(1));
    }

    static string EncodeSymbol(string symbol)
    {
        int index = GetSymbolIndex(symbol);
        int zeroCount = 2 * index + 4;
        return $"10^{zeroCount}";
    }

    static string EncodeState(string state)
    {
        int index = GetStateIndex(state);
        int zeroCount = 2 * index + 5;
        return $"10^{zeroCount}";
    }
        
    static string EncodeAction(char action)
    {
        switch (action)
        {
            case 'R':
                return "10^1";
            case 'L':
                return "10^2";
            case 'E':
                return "10^3";
            default:
                throw new ArgumentException("Invalid action");
        }
    }

    static string EncodeCommand(TuringMachineCommand command)
    {
        StringBuilder code = new StringBuilder();
        code.Append(EncodeSymbol(command.CurrentSymbol));
        code.Append(" ");
        code.Append(EncodeState(command.CurrentState));
        code.Append(" ");
        code.Append(EncodeSymbol(command.NewSymbol));
        code.Append(" ");
        code.Append(EncodeAction(command.Action));
        code.Append(" ");
        code.Append(EncodeState(command.NewState));
        code.Append(" ");
        return code.ToString();
    }
    static string EncodeSymbol(int value)
    {
        if (value < 4 && value % 2 != 0)
            TuringMachhineNumberExceprion();
        int index = (value - 4) / 2;
        return $"a{(index):0}";
    }

    static string EncodeState(int value)
    {
        if (value < 5 && value % 2 == 0)
            TuringMachhineNumberExceprion();
        int index = (value - 5) / 2;
        return $"q{(index):0}";
    }

    static char EncodeAction(int value)
    {
        if (value < 4 && value > 0)
        {
            switch (value)
            {
                case 1:
                    return 'R';
                case 2:
                    return 'L';
                case 3:
                    return 'E';
                default:
                    throw new ArgumentException("Invalid action");
            }
        }
        else
        {
            TuringMachhineNumberExceprion();
            return '0';
        }                    
    }

    static List<TuringMachineCommand> DecodeBinaryNumber(string filename)
    {
        List<TuringMachineCommand> commands = new List<TuringMachineCommand>();
        TuringMachineCommand currentCommand = new TuringMachineCommand();        

        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] binaryParts = line.Split(' ');

                if (binaryParts.Length % 5 != 0)                 
                    TuringMachhineNumberExceprion();


                int numberCommands = binaryParts.Length / 5;

                for (int i = 0; i < numberCommands; i++)
                {
                    int CurrentSymbol = int.Parse(binaryParts[i*5+0].Replace("10^", ""));
                    currentCommand.CurrentSymbol = EncodeSymbol(CurrentSymbol);

                    int CurrentState = int.Parse(binaryParts[i * 5 + 1].Replace("10^", ""));
                    currentCommand.CurrentState = EncodeState(CurrentState);

                    int NewSymbol = int.Parse(binaryParts[i * 5 + 2].Replace("10^", ""));
                    currentCommand.NewSymbol = EncodeSymbol(NewSymbol);

                    int Action = int.Parse(binaryParts[i * 5 + 3].Replace("10^", ""));
                    currentCommand.Action = EncodeAction(Action);

                    int NewState = int.Parse(binaryParts[i * 5 + 4].Replace("10^", ""));
                    currentCommand.NewState = EncodeState(NewState);

                    commands.Add(currentCommand);
                    currentCommand = new TuringMachineCommand();
                }

            }
        }

        return commands;
    }
    static void PrintTuringMachineNumber(List<TuringMachineCommand> machine)
    {
        Console.WriteLine("Номер машины Тьюринга:");
        StringBuilder binaryCode = new StringBuilder();
        foreach (var command in machine)
        {
            binaryCode.Append(EncodeCommand(command));
        }
        Console.WriteLine(binaryCode.ToString() + "\n");
    }

    static void PrintTuringMachine(List<TuringMachineCommand> commands)
    {
        Console.WriteLine("Машина Тьюринга по номеру:");
        foreach (var command in commands)
        {
            Console.WriteLine($"{command.CurrentSymbol}{command.CurrentState}\t{command.NewSymbol}{command.Action}{command.NewState}");
        }
    }
    
    static void TuringMachhineNumberExceprion()
    {
        Console.WriteLine("Некорректный формат номера машины Тьюринга");
        Environment.Exit(0);
    }

    static void Main(string[] args)
    {
        List<TuringMachineCommand> machine = ReadCommandsFromFile("C:\\Уник\\6 семак\\ТА\\Turing machine number\\machine2.txt");
        PrintTuringMachineNumber(machine);
        string filename = "C:\\Уник\\6 семак\\ТА\\Turing machine number\\number2.txt";
        List<TuringMachineCommand> commands = DecodeBinaryNumber(filename);
        PrintTuringMachine(commands);
    }

}
