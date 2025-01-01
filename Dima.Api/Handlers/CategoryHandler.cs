using Dima.Api.Data;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class CategoryHandler(AppDbContext dbContext) : ICategoryHandler
{
    public async Task<BaseResponse<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        try
        {
            var category = new Category
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description
            };
        
            dbContext.Categories.Add(category);
        
            await dbContext.SaveChangesAsync();
        
            return new BaseResponse<Category?>(category, 201, "Categoria criada com sucesso.");
        }
        catch
        {
            return new BaseResponse<Category?>(null, 500, "Não foi possível criar a categoria.");
        }
    }

    public async Task<BaseResponse<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        try
        {
            var category = await dbContext
                .Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
        
            if (category is null)
                return new BaseResponse<Category?>(null, 404, "Categoria não encontrada.");
        
            category.Title = request.Title;
            category.Description = request.Description;

            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();
            
            return new BaseResponse<Category?>(category);
        }
        catch
        {
            return new BaseResponse<Category?>(null, 500, "Não foi possível atualizar a categoria.");
        }
    }

    public async Task<BaseResponse<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        try
        {
            var category = await dbContext
                .Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
        
            if (category is null)
                return new BaseResponse<Category?>(null, 404, "Categoria não encontrada.");
        
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
        
            return new BaseResponse<Category?>(category, message: "Categoria deletada com sucesso.");
        }
        catch
        {
            return new BaseResponse<Category?>(null, 500, "Não foi possível deletar a categoria.");
        }
    }

    public async Task<BaseResponse<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        try
        {
            var category = await dbContext
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
        
            return category is null
                ? new BaseResponse<Category?>(null, 404, "Categoria não encontrada.")
                : new BaseResponse<Category?>(category);
        }
        catch
        {
            return new BaseResponse<Category?>(null, 500, "Não foi possível buscar a categoria.");
        }
    }

    public async Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request)
    {
        
        try
        {
            var query = dbContext.Categories
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Title);
            
            var categories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query
                .CountAsync();
            
            return new PagedResponse<List<Category>>(categories, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Category>>(null, 500, "Não foi possível consultar as categorias.");
        }
    }
}