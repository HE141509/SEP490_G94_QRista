using QRB.Models;

namespace QRB.Services
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetAllIngredientsAsync();
        Task<List<NguyenLieu>> GetAllIngredientsAsNguyenLieuAsync();
        Task<Ingredient?> GetIngredientByIdAsync(Guid id);
        Task<NguyenLieu?> GetIngredientByIdAsNguyenLieuAsync(Guid id);
        Task<Ingredient> AddIngredientAsync(Ingredient ingredient);
        Task<NguyenLieu> AddIngredientAsync(NguyenLieu nguyenLieu);
        Task<bool> UpdateIngredientAsync(Ingredient ingredient);
        Task<bool> UpdateIngredientAsync(NguyenLieu nguyenLieu);
        Task<bool> DeleteIngredientAsync(Guid id);
        Task<bool> IngredientCodeExistsAsync(string ingredientCode, Guid? excludeId = null);
    }
}
