﻿namespace Dorisoy.PanClient.Commands;

public class MoveDocumentCommand
{
    public Guid DocumentId { get; set; }
    public Guid DestinationFolderId { get; set; }
}
