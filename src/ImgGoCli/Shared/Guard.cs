using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System;

internal static class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ArgumentNotNull<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string? name = null)
        where T : class
    {
        return value ?? throw new ArgumentNullException(name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ArgumentNotNullOrEmpty(string? value,
        [CallerArgumentExpression(nameof(value))]
        string? name = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }

        if (value.Length == 0)
        {
            throw new ArgumentException("Argument value cannot be empty string.", name);
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ArgumentNotNullOrWhiteSpace(string? value,
        [CallerArgumentExpression(nameof(value))]
        string? name = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Argument value cannot be white space string.", name);
        }

        return value;
    }
}