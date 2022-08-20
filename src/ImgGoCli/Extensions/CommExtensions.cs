using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ImgGoCli.Extensions;

public static class CommExtensions
{
    /// <summary>
    /// String为Null或者空白字符
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)]this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// String为Null或者空字符
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)]this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// String不为Null或者空白字符
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrWhiteSpace([NotNullWhen(true)]this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// String不为Null或者空字符
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty([NotNullWhen(true)]this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

}