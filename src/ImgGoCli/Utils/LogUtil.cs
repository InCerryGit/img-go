using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ImgGoCli.Utils;

public static class LogUtil
{
    [Conditional("DEBUG")]
    public static void Debug(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string msg)
    {
        Console.WriteLine(msg);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Notify(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
}