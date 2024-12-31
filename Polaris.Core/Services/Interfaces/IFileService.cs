namespace Polaris.Core.Services.Interfaces;

public interface IFileService
{
    string ReadFile(string filePath);
    void SaveFile(string filePath, string content);
}
