using Dima.Api.Data;
using Dima.Core.Common;
using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class TransactionHandler(AppDbContext dbContext) : ITransactionHandler
{
    public async Task<BaseResponse<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        var transaction = new Transaction
        {
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.Now,
            Amount = request.Amount,
            PaidOrReceivedAt = request.PaidOrReceivedAt,  
            Title = request.Title,
            Type = request.Type
        };
        
        await dbContext.Transactions.AddAsync(transaction);
        await dbContext.SaveChangesAsync();
        
        return new BaseResponse<Transaction?>(transaction, 201, "Transação criada com sucesso.");
    }

    public async Task<BaseResponse<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        try
        {
            var transaction = await dbContext
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            
            if (transaction is null)
                return new BaseResponse<Transaction?>(null, 404, "Transação não encontrada.");
            
            transaction.CategoryId = request.CategoryId;
            transaction.Amount = request.Amount;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;
            transaction.Title = request.Title;
            transaction.Type = request.Type;
            
            dbContext.Transactions.Update(transaction);
            await dbContext.SaveChangesAsync();
            
            return new BaseResponse<Transaction?>(transaction);
        }
        catch
        {
            return new BaseResponse<Transaction?>(null, 500, "Erro ao atualizar transação.");
        }
    }

    public async Task<BaseResponse<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await dbContext
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            
            if (transaction is null)
                return new BaseResponse<Transaction?>(null, 404, "Transação não encontrada.");
            
            dbContext.Transactions.Remove(transaction);
            await dbContext.SaveChangesAsync();
            
            return new BaseResponse<Transaction?>(transaction);
        }
        catch
        {
            return new BaseResponse<Transaction?>(null, 500, "Erro ao deletar transação.");
        }
    }

    public async Task<BaseResponse<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
    {
        try
        {
            var transaction = await dbContext
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
        
            return transaction is null 
                ? new BaseResponse<Transaction?>(null, 404, "Transação não encontrada.") 
                : new BaseResponse<Transaction?>(transaction);
        }
        catch
        {
            return new BaseResponse<Transaction?>(null, 500, "Erro ao buscar transação.");
        }
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDay();
            request.EndDate ??= DateTime.Now.GetLastDay();
        }
        catch
        {
            return new PagedResponse<List<Transaction>?>(null, 500, "Não foi possível determinar a data de início ou fim.");
        }

        try
        {
            var query = dbContext
                .Transactions
                .AsNoTracking()
                .Where(
                    x => x.UserId == request.UserId &&
                         x.CreatedAt >= request.StartDate &&
                         x.CreatedAt <= request.EndDate)
                .OrderByDescending(x => x.CreatedAt);

            var transactions = await query
                .Skip(request.PageSize)
                .Take((request.PageNumber - 1) * request.PageSize)
                .ToListAsync();
        
            var count = await query.CountAsync();
        
            return new PagedResponse<List<Transaction>?>(
                transactions, 
                count,
                request.PageNumber,
                request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Transaction>?>(null, 500, "Erro ao buscar as transações.");
        }
    }
}