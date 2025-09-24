using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Services
{
    public class DepartmentService
    {
        private readonly QRBDbContext _context;

        public DepartmentService(QRBDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách departments (trả về ChiNhanh cho backward compatibility)
        public async Task<List<ChiNhanh>> GetAllDepartmentsAsChiNhanhAsync()
        {
            var departments = await _context.Departments
                .Where(d => !d.IsDelete)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return departments.Select(d => new ChiNhanh(d)).ToList();
        }

        // Lấy danh sách departments (trả về Department)
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDelete)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        // Lấy department theo ID (trả về ChiNhanh)
        public async Task<ChiNhanh?> GetDepartmentAsChiNhanhByIdAsync(Guid id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.ID == id && !d.IsDelete);

            if (department == null)
                return null;

            return new ChiNhanh(department);
        }

        // Lấy department theo ID (trả về Department)
        public async Task<Department?> GetDepartmentByIdAsync(Guid id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.ID == id && !d.IsDelete);
        }

        // Thêm department mới (nhận ChiNhanh, lưu vào Department)
        public async Task<Guid> AddDepartmentAsync(ChiNhanh chiNhanh)
        {
            var department = chiNhanh.ToDepartment();
            department.ID = Guid.NewGuid();
            department.CreateTime = DateTime.Now;

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department.ID;
        }

        // Thêm department mới (parameters)
        public async Task<Guid> AddDepartmentAsync(string departmentName, string departmentCode)
        {
            var department = new Department
            {
                ID = Guid.NewGuid(),
                DepartmentName = departmentName,
                DepartmentCode = departmentCode,
                IsDelete = false,
                CreateTime = DateTime.Now
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department.ID;
        }

        // Cập nhật department (nhận ChiNhanh)
        public async Task<bool> UpdateDepartmentAsync(ChiNhanh chiNhanh)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.ID == chiNhanh.ID && !d.IsDelete);

            if (department == null)
                return false;

            department.DepartmentName = chiNhanh.TenChiNhanh;
            department.DepartmentCode = chiNhanh.MaChiNhanh;
            department.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // Cập nhật department (parameters)
        public async Task<bool> UpdateDepartmentAsync(Guid id, string departmentName, string departmentCode)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.ID == id && !d.IsDelete);

            if (department == null)
                return false;

            department.DepartmentName = departmentName;
            department.DepartmentCode = departmentCode;
            department.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa department (soft delete)
        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.ID == id && !d.IsDelete);

            if (department == null)
                return false;

            department.IsDelete = true;
            department.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        // Kiểm tra department có tồn tại không
        public async Task<bool> DepartmentExistsAsync(Guid id)
        {
            return await _context.Departments
                .AnyAsync(d => d.ID == id && !d.IsDelete);
        }

        // Kiểm tra department code có tồn tại không
        public async Task<bool> DepartmentCodeExistsAsync(string departmentCode, Guid? excludeId = null)
        {
            var query = _context.Departments
                .Where(d => d.DepartmentCode == departmentCode && !d.IsDelete);

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.ID != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
