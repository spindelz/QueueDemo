using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("queue")]
public class Queue
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("number")]
    [StringLength(2)]
    public string Number { get; set; } = string.Empty;

    [Column("qDate")]
    public DateTime QDate { get; set; } = DateTime.Now;
}

public class QueueResponse
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string QDate { get; set; } = string.Empty;
}