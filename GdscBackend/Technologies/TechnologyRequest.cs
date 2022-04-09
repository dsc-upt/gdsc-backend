namespace GdscBackend.RequestModels;

public class TechnologyRequest : Request
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string IconId { get; set; }
}