namespace TestApplication.Models;

/// <summary>
/// Человек
/// </summary>
public class Person
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime BirthDay { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    public SexType Sex { get; set; }

    /// <summary>
    /// Гражданство
    /// </summary>
    public string Citizenship { get; set; } = null!;

    /// <summary>
    /// Семья
    /// </summary>
    public List<string>? Family { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime DateCreateUtc { get; set; }

    /// <summary>
    /// Пользователь создавший документ
    /// </summary>
    public string? UserCreate { get; set; }
    
    /// <summary>
    /// Дата изменения
    /// </summary>
    public DateTime DateUpdateUtc { get; set; }

    /// <summary>
    /// Пользователь изменивший документ
    /// </summary>
    public string? UserUpdate { get; set; }
}