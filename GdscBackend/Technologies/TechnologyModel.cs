namespace GdscBackend.Models;

public class TechnologyModel : Model
{
    public string Name { get; set; }
    public string Description { get; set; }
    public FileModel Icon { get; set; }
}