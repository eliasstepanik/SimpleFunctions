using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Functions.Data;

public class Instance
{
    public Instance(string name, string instanceId)
    {
        Name = name;
        InstanceId = instanceId;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string InstanceId { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public int FunctionId { get; set; }
    public Function Function { get; set; }
}