namespace FF.Articles.Backend.AI.API.Models.Entities;

public class SystemMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
