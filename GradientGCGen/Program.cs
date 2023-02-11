using System.Drawing;
using System.Text.RegularExpressions;

internal class Program
{
    public static void Main(string[] args)
    {
        var text = GetInput("Put your text to apply linear gradient on", x => !string.IsNullOrEmpty(x));
        if (string.IsNullOrEmpty(text))
            text = "Fuck you!";

        var colorA = GetInput("Put a HEX of a color A", IsHexColorRight);
        if (string.IsNullOrEmpty(colorA))
            colorA = "EBACCA";

        if (text.Length == 1)
        {
            Console.WriteLine(GetPrintChar(text[0], colorA));
            return;
        }

        var colorB = GetInput("Put a HEX of a color B", IsHexColorRight);
        if (string.IsNullOrEmpty(colorB))
            colorB = "FFAB00";

        if (text.Length == 2)
        {
            Console.WriteLine(GetPrintChar(text[0], colorA) + GetPrintChar(text[1], colorB));
            return;
        }

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