using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Functions.Data;

public class Function
{
    public Function(string name, string imageTag)
    {
        Name = name;
        ImageTag = imageTag;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string ImageTag { get; set; }
    
    public List<Environment> EnvironmentVariables { get; set; } = null!;

    public List<Instance> Instances { get; set; } = null!;
}