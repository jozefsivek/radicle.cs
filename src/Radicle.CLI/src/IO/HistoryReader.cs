namespace Radicle.CLI.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Collection of history reading APIs.
/// </summary>
public static class HistoryReader
{
    /// <summary>
    /// Gets default history file path.
    /// </summary>
    /// <param name="pathRelativeToHome">Path of the history file
    ///     relative to user home directory.</param>
    /// <returns>Path to default history file.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string GetFilePath(string pathRelativeToHome)
    {
        string userPath = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile);

        return Path.Combine(userPath, pathRelativeToHome);
    }

    /// <summary>
    /// Try to read the history file at <paramref name="filePath"/>.
    /// Fails silently in case the file can not be read or accessed.
    /// </summary>
    /// <param name="filePath">Path to read file from.</param>
    /// <returns>Collection of history items. Can be empty.
    /// First item is the oldest.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="PathTooLongException">Thrown
    ///     if <paramref name="filePath"/> is too long.</exception>
    /// <exception cref="NotSupportedException">Thrown
    ///     if <paramref name="filePath"/> is in invalid format.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="filePath"/> is empty.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="filePath"/> contains only white spaces.</exception>
    public static async Task<ICollection<string>> TryReadHistoryAsync(string filePath)
    {
        Ensure.Param(filePath).NotEmpty().NotWhiteSpace().Done();

        try
        {
            using StreamReader stream = File.OpenText(filePath);

            return await TryReadHistoryAsync(stream).ConfigureAwait(false);
        }
        catch (DirectoryNotFoundException)
        {
            return Array.Empty<string>();
        }
        catch (FileNotFoundException)
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Try to write the history file at <paramref name="filePath"/>.
    /// Fails silently in case the file can not be read or accessed.
    /// </summary>
    /// <param name="filePath">Path to write file to.</param>
    /// <param name="history">History to write.</param>
    /// <returns><see langword="true"/> if succesfully written.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="PathTooLongException">Thrown
    ///     if <paramref name="filePath"/> is too long.</exception>
    /// <exception cref="NotSupportedException">Thrown
    ///     if <paramref name="filePath"/> is in invalid format.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="filePath"/> is empty.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="filePath"/> contains only white spaces.</exception>
    public static async Task<bool> TryWriteHistoryAsync(
            string filePath,
            IEnumerable<string> history)
    {
        Ensure.Param(filePath).NotEmpty().NotWhiteSpace().Done();

        try
        {
            using StreamWriter stream = File.CreateText(filePath);

            await TryWriteHistoryAsync(stream, history).ConfigureAwait(false);

            return true;
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (DirectoryNotFoundException)
        {
        }

        return false;
    }

    /// <summary>
    /// Try to append the history file at <paramref name="filePath"/>.
    /// Fails silently in case the file can not be read or accessed.
    /// </summary>
    /// <param name="filePath">Path to write file to.</param>
    /// <param name="historyPatch">History to write.</param>
    /// <returns><see langword="true"/> if succesfully written.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="PathTooLongException">Thrown
    ///     if <paramref name="filePath"/> is too long.</exception>
    /// <exception cref="NotSupportedException">Thrown
    ///     if <paramref name="filePath"/> is in invalid format.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="filePath"/> is empty.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="filePath"/> contains only white spaces.</exception>
    public static async Task<bool> TryAppendHistoryAsync(
            string filePath,
            IEnumerable<string> historyPatch)
    {
        Ensure.Param(filePath).NotEmpty().NotWhiteSpace().Done();

        try
        {
            using StreamWriter stream = File.AppendText(filePath);

            await TryWriteHistoryAsync(stream, historyPatch).ConfigureAwait(false);

            return true;
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (DirectoryNotFoundException)
        {
        }

        return false;
    }

    /// <summary>
    /// Try to read the history from the stream.
    /// Fails silently in case the file can not be read or accessed.
    /// </summary>
    /// <param name="stream">Text stream.</param>
    /// <returns>Collection of history items. Can be empty.</returns>
    /// <exception cref="ArgumentNullException">Throw if
    ///     required parameter is <see langword="null"/>.</exception>
    public static async Task<ICollection<string>> TryReadHistoryAsync(
            TextReader stream)
    {
        Ensure.Param(stream).Done();

        List<string> result = new();

        while (true)
        {
            try
            {
                string? line = await stream.ReadLineAsync().ConfigureAwait(false);

                if (line is null)
                {
                    break;
                }

                result.Add(line);
            }
            catch (ArgumentOutOfRangeException)
            {
                // skip this line as it is too long
            }
        }

        return result;
    }

    /// <summary>
    /// Try to read the history from the stream.
    /// Fails silently in case the file can not be read or accessed..
    /// </summary>
    /// <param name="stream">Text stream to write to.</param>
    /// <param name="history">History to append.</param>
    /// <returns>Collection of history items. Can be empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static async Task TryWriteHistoryAsync(
            TextWriter stream,
            IEnumerable<string> history)
    {
        Ensure.Param(stream).Done();
        Ensure.Param(history).AllNotNull().Done();

        foreach (string item in history)
        {
            await stream.WriteLineAsync(item).ConfigureAwait(false);
        }
    }
}
