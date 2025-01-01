using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Categories;

public class UpdateCategoryRequest : BaseRequest
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Título Inválido")]
    [MaxLength(80, ErrorMessage = "Título deve ter no máximo 80 caracteres")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Descrição Inválida")]
    public string Description { get; set; } = string.Empty;
}