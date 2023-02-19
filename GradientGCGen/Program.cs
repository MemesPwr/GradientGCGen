using System.Drawing;
using System.Text.RegularExpressions;

internal class Program
{
    private static readonly string CurrentBinaryName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".exe";
    
    public static void Main(string[] args)
    {
        string? text, colorA, colorB;
        if (args.Length == 1 && (args.ElementAt(0).Equals("/?", StringComparison.OrdinalIgnoreCase) || args.ElementAt(0).Equals("-halp", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"{CurrentBinaryName} <color A> <color B> <your text>");
            return;
        }
        if (args.Length >= 3)
        {
            colorA = args.ElementAt(0);
            colorB = args.ElementAt(1);

            if (!IsHexColorRight(colorA))
            {
                Console.WriteLine($"\"{colorA}\" - is not looks like HEX color...");
                return;
            }

            if (!IsHexColorRight(colorB))
            {
                Console.WriteLine($"\"{colorB}\" - is not looks like HEX color...");
                return;
            }

            text = string.Join(' ', args.Skip(2)).Trim();
            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine("What a hell... Text to color is empty!");
                return;
            }

            Generate(colorA, colorB, text);
            return;
        }

        text = GetInput("Put your text to apply linear gradient on", x => !string.IsNullOrEmpty(x));
        if (string.IsNullOrEmpty(text))
            text = "Fuck you!";

        colorA = GetInput("Put a HEX of a color A", IsHexColorRight);
        if (string.IsNullOrEmpty(colorA))
            colorA = "EBACCA";

        if (text.Length == 1)
        {
            Console.WriteLine(GetPrintChar(text[0], colorA));
            return;
        }

        colorB = GetInput("Put a HEX of a color B", IsHexColorRight);
        if (string.IsNullOrEmpty(colorB))
            colorB = "FFAB00";

        if (text.Length == 2)
        {
            Console.WriteLine(GetPrintChar(text[0], colorA) + GetPrintChar(text[1], colorB));
            return;
        }

        Generate(colorA, colorB, text);
    }

    private static void Generate(string colorA, string colorB, string text)
    {
        var gradientStart = ParseColorFromHex(colorA);
        var gradientStop = ParseColorFromHex(colorB);
        Console.WriteLine(string.Join(string.Empty, GetGradients(gradientStart, gradientStop, text.Length).Select((col, idx) => GetPrintChar(text[idx], col))));
    }

    private static Color ParseColorFromHex(string hex)
        => ColorTranslator.FromHtml(hex.StartsWith('#') ? hex : $"#{hex}");

    private static IEnumerable<Color> GetGradients(Color start, Color end, int steps)
    {
        int stepA = ((end.A - start.A) / (steps - 1));
        int stepR = ((end.R - start.R) / (steps - 1));
        int stepG = ((end.G - start.G) / (steps - 1));
        int stepB = ((end.B - start.B) / (steps - 1));

        for (int i = 0; i < steps; i++)
        {
            yield return Color.FromArgb(start.A + (stepA * i),
                start.R + (stepR * i),
                start.G + (stepG * i),
                start.B + (stepB * i));
        }
    }

    static string GetPrintChar(char ch, Color color)
        => GetPrintChar(ch, ColorTranslator.ToHtml(color));

    static string GetPrintChar(char ch, string hexColor)
        => $"<color=\"{(hexColor.StartsWith('#') ? hexColor : $"#{hexColor}")}\">{ch}</color>";

    static bool IsHexColorRight(string? input)
        => Regex.IsMatch(input ?? string.Empty, @"^\#?([\dA-Fa-f]{2}){3,4}$");

    static string? GetInput(string message, Func<string?, bool> validCheckFn)
    {
        while (true)
        {
            Console.WriteLine(message);
            Console.Write("> ");

            var input = Console.ReadLine();
            if (validCheckFn(input))
                return input;
        }
    }
}