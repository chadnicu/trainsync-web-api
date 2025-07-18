namespace TrainSyncAPI.Dtos;

public class TemplateBaseViewModel
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
}
