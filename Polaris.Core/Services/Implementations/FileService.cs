using Polaris.Core.Services.Interfaces;

namespace Polaris.Core.Services.Implementations;

public sealed class FileService : IFileService
{
    public string ReadFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.");
        }

        return File.ReadAllText(filePath);
    }

    public void SaveFile(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty space.");
        }

        File.WriteAllText(filePath, content);
    }
}
