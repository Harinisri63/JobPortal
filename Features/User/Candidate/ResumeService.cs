using JobPortal.Shared.Exceptions;

namespace JobPortal.Features.Candidate;

internal class ResumeService
{
    private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase){ ".pdf", ".doc", ".docx" };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public static string UploadResume(string filePath, string seekerId)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ValidationException("FilePath", "File path is required.");

        string ext = Path.GetExtension(filePath);
        if (!_allowedExtensions.Contains(ext))
            throw new ValidationException("FileFormat", "Supported formats: PDF, DOC, DOCX.");

        if (File.Exists(filePath))
        {
            var info = new FileInfo(filePath);
            if (info.Length > MaxFileSizeBytes)
                throw new ValidationException("FileSize", "File size must not exceed 5MB.");
        }

        string storageDir = Path.Combine(AppContext.BaseDirectory, "resumes", seekerId);
        Directory.CreateDirectory(storageDir);
        string destFile = Path.Combine(storageDir, $"resume_{DateTime.Now:yyyyMMddHHmmss}{ext}");

        if (File.Exists(filePath))
            File.Copy(filePath, destFile, overwrite: true);
        else
            destFile = filePath;

        return destFile;
    }

    public static bool HasResume(string resumeUrl) => !string.IsNullOrWhiteSpace(resumeUrl);

    public static void ValidateResumePresent(string resumeUrl)
    {
        if (!HasResume(resumeUrl)) 
            throw new ResumeNotUploadedException();
    }
}
