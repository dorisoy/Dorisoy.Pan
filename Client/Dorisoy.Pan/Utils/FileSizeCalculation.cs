namespace Dorisoy.Pan;

public static class DirectorySizeCalculation
{
    public static long DirectorySize(DirectoryInfo dInfo, bool includeSubDir)
    {
        // Enumerate all the files
        long totalSize = dInfo.EnumerateFiles()
                     .Sum(file => file.Length);

        // If Subdirectories are to be included
        if (includeSubDir)
        {
            // Enumerate all sub-directories
            totalSize += dInfo.EnumerateDirectories()
                     .Sum(dir => DirectorySize(dir, true));
        }
        return totalSize;
    }
}
