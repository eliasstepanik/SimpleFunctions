using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Functions.Data;

public class Environment
{
    public Environment(string key, string value)
    {
        Key = key;
        Value = value;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Key { get; set; }
    [Required]
    public string Value { get; set; }
    
    public int FunctionId { get; set; }
    public Function Function { get; set; } = null!;
}