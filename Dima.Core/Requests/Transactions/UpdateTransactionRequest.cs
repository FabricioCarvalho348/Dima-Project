using System.ComponentModel.DataAnnotations;
using Dima.Core.Enums;

namespace Dima.Core.Requests.Transactions;

public class UpdateTransactionRequest : BaseRequest
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Título Inválido")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tipo Inválido")]
    public ETransactionType Type { get; set; }
    
    [Required(ErrorMessage = "Valor Inválido")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Categoria Inválida")]
    public long CategoryId { get; set; }

    [Required(ErrorMessage = "Conta Inválida")]
    public DateTime? PaidOrReceivedAt { get; set; }
}