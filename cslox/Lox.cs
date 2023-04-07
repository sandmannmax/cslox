namespace cslox;

public class Lox
{
  static bool _hadError = false;

  static void RunFile(string name)
  {
    string code = File.ReadAllText(name);
    Run(code);
    if (_hadError) Environment.Exit(69);
  }

  static void RunPrompt()
  {
    while (true)
    {
      Console.Write("> ");
      var code = Console.ReadLine();
      if (code != null)
      {
        if (code == "exit") return;
        Run(code);
      }
    }
  }

  static void Run(string code)
  {
    Scanner scanner = new Scanner(code);
    var tokens = scanner.ScanTokens();
    tokens.ForEach(t => Console.WriteLine(t));
  }

  public static void Error(int line, string message)
  {
    Report(line, "", message);
  }

  static void Report(int line, string where, string message)
  {
    Console.WriteLine($"[line {line}] Error{where}: {message}");
    _hadError = true;
  }

  public static void Run(string[] args)
  {
    Console.WriteLine("Welcome to cslox.");

    if (args.Length > 1)
    {
      Console.WriteLine("Usage: cslox [script]");
    }
    else if (args.Length == 1)
    {
      RunFile(args[0]);
    }
    else
    {
      RunPrompt();
    }
  }
}