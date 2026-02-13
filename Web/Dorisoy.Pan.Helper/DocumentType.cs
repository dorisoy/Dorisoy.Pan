namespace Dorisoy.Pan.Helper
{
    public static class DocumentType
    {
        public static string GetDocumentType(string extension)
        {
            string type = string.Empty;
            switch (extension)
            {

                case ".pdf":
                    type = "pdf";
                    break;
                case ".docx":
                    type = "mammoth";
                    break;
                case ".pptx":
                    type = "office";
                    break;
                default:
                    type = "";
                    break;
            }
            return type;
        }

        public static string Get64ContentStartText(string extension)
        {
            string type = string.Empty;
            switch (extension)
            {

                case ".pdf":
                    type = "data:application/pdf;base64,";
                    break;
                case ".docx":
                    type = "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,";
                    break;
                case ".pptx":
                    type = "data:application/vnd.openxmlformats-officedocument.presentationml.presentation;base64,";
                    break;
                default:
                    type = "";
                    break;
            }
            return type;
        }
    }
}
