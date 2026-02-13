using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class EmailTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
