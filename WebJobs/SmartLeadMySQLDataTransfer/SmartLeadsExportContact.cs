using System;

namespace SmartLeadsDataTransfer;

public class SmartLeadsExportedContact
{
    public int Id { get; set; } // Primary Key (auto-increment in the database)

    public DateTime? ExportedDate { get; set; } // Nullable DateTime

    public string? Email { get; set; } // Max length 255 in the database

    public string? ContactSource { get; set; } // Max length 255 in the database

    public int? Rate { get; set; } // Nullable integer

    public short? HasReply { get; set; } // Nullable smallint (mapped to short in C#)

    public DateTime? ModifiedAt { get; set; } // Nullable DateTime

    public string? Category { get; set; } // Max length 255 in the database

    public string? MessageHistory { get; set; } // JSON data stored as string

    public string? LatestReplyPlainText { get; set; } // Large text field

    public short? HasReviewed { get; set; } // Nullable smallint (mapped to short in C#)

    public int? SmartleadId { get; set; } // Nullable integer

    public DateTime? ReplyDate { get; set; } // Nullable DateTime

    public DateTime? RepliedAt { get; set; } // Nullable DateTime

    public bool? FailedDelivery { get; set; } // Nullable boolean (mapped to BIT in SQL Server)

    public bool? RemovedFromSmartleads { get; set; } // Nullable boolean (mapped to BIT in SQL Server)

    public string? SmartLeadsStatus { get; set; } // Max length 50 in the database

    public string? SmartLeadsCategory { get; set; } // Max length 100 in the database
}
