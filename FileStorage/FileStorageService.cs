using System.Text.Json;

namespace JobPortal.FileStorage;

internal class FileStorageService<T>
{
    private readonly string _filePath;

    public FileStorageService(string fileName)
    {
        string folderPath = @"D:\Synergech\Project\Verify\A\Files";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        _filePath = Path.Combine(folderPath, fileName);
    }

    public bool FileExists()
    {
        return File.Exists(_filePath);
    }

    public void SaveData(List<T> data)
    {
        string json = JsonSerializer.Serialize(data,new JsonSerializerOptions{WriteIndented = true,IncludeFields = true});
        File.WriteAllText(_filePath, json);
    }

    public List<T> LoadData()
    {
        if (!File.Exists(_filePath))
            return new List<T>();
        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json,new JsonSerializerOptions{IncludeFields = true}) ?? new List<T>();
    }
}