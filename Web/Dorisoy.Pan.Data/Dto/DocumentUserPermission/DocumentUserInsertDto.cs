using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.Data.Dto
{
    public class DocumentUserInsertDto
    {
        public Guid Id { get; set; }
        public List<string> Users { get; set; }
    }
}
