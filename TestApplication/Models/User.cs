namespace TestApplication.Models;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public List<Guid>? ListGuid { get; set; }
}