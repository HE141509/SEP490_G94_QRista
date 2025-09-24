using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly QRBDbContext _context;

        public IngredientService(QRBDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.IngredientName)
                .ToListAsync();
        }

        public async Task<List<NguyenLieu>> GetAllIngredientsAsNguyenLieuAsync()
        {
            var ingredients = await GetAllIngredientsAsync();
            return ingredients.Select(i => new NguyenLieu(i)).ToList();
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(Guid id)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.ID == id && !i.IsDeleted);
        }

        public async Task<NguyenLieu?> GetIngredientByIdAsNguyenLieuAsync(Guid id)
        {
            var ingredient = await GetIngredientByIdAsync(id);
            return ingredient != null ? new NguyenLieu(ingredient) : null;
        }

        public async Task<Ingredient> AddIngredientAsync(Ingredient ingredient)
        {
            ingredient.ID = Guid.NewGuid();
            ingredient.CreateTime = DateTime.Now;
            ingredient.IsDeleted = false;

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<NguyenLieu> AddIngredientAsync(NguyenLieu nguyenLieu)
        {
            var ingredient = nguyenLieu.ToIngredient();
            var addedIngredient = await AddIngredientAsync(ingredient);
            return new NguyenLieu(addedIngredient);
        }

        public async Task<bool> UpdateIngredientAsync(Ingredient ingredient)
        {
            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.ID == ingredient.ID && !i.IsDeleted);

            if (existingIngredient == null)
                return false;

            existingIngredient.IngredientName = ingredient.IngredientName;
            existingIngredient.IngredientCode = ingredient.IngredientCode;
            existingIngredient.UnitOfMeasure = ingredient.UnitOfMeasure;
            existingIngredient.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateIngredientAsync(NguyenLieu nguyenLieu)
        {
            var ingredient = nguyenLieu.ToIngredient();
            return await UpdateIngredientAsync(ingredient);
        }

        public async Task<bool> DeleteIngredientAsync(Guid id)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.ID == id && !i.IsDeleted);

            if (ingredient == null)
                return false;

            ingredient.IsDeleted = true;
            ingredient.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IngredientCodeExistsAsync(string ingredientCode, Guid? excludeId = null)
        {
            var query = _context.Ingredients
                .Where(i => i.IngredientCode == ingredientCode && !i.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(i => i.ID != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
