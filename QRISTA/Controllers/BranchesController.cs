using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRB.Data;

namespace QRB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly QRBDbContext _context;

        public BranchesController(QRBDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var branches = await _context.Departments
                    .Where(b => !b.IsDelete)
                    .Select(b => new
                    {
                        id = b.ID,
                        tenChiNhanh = b.DepartmentName,
                        maChiNhanh = b.DepartmentCode
                    })
                    .OrderBy(b => b.tenChiNhanh)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = branches,
                    message = "Lấy danh sách chi nhánh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy danh sách chi nhánh: " + ex.Message
                });
            }
        }
    }
}
