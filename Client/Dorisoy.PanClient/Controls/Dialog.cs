
using Avalonia.Platform.Storage;

namespace Dorisoy.PanClient.Controls;
public static class Dialog
{
    public static async void Save(string title, string defaultFilename, Action<string> saveAction)
    {
        var filename = await Save(title, defaultFilename);
        if (filename != null)
            saveAction(filename);
    }

    public static async Task<string> Save(string title, string defaultFilename)
    {
        try
        {
            //var storageProvider = Locator.Current.GetService<IStorageProvider>();
            var storageProvider = App.MainView.GetStorageProvider();
            var option = new FilePickerSaveOptions()
            {
                Title = title,
                FileTypeChoices = new FilePickerFileType[]
                {
                new FilePickerFileType("PDF files (.pdf)"){ Patterns = new List<string> { "pdf" } },
                new FilePickerFileType("Excel files (.xlsx)"){ Patterns = new List<string> { "xlsx", "Cvs" } },
                new FilePickerFileType("All files (.*)"){ Patterns = new List<string> { "mp4", "jepg", "*" } }
                },
                SuggestedFileName = defaultFilename
            };
            if (storageProvider.CanSave)
            {
                var saveDialog = await storageProvider.SaveFilePickerAsync(option);
                if (saveDialog != null)
                    return saveDialog.TryGetLocalPath();
                else
                    return "";
            }
            else
            {
                return "";
            }
        }
        catch (Exception)
        {
            return "";
        }
    }
}
