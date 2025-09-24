using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Controllers
{
    [Route("api/user-branches")]
    [ApiController]
    public class UserBranchesController : ControllerBase
    {
        private readonly QRBDbContext _context;

        public UserBranchesController(QRBDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserBranches(Guid userId)
        {
            try
            {
                var userBranches = await _context.UserBranches
                    .Where(ub => ub.UserId == userId)
                    .Select(ub => new
                    {
                        id = ub.Id,
                        userId = ub.UserId,
                        branchId = ub.BranchId,
                        isActive = ub.IsActive,
                        assignedDate = ub.AssignedDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = userBranches,
                    message = "Lấy danh sách chi nhánh của người dùng thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignBranches([FromBody] AssignBranchRequest request)
        {
            if (request == null || request.UserId == Guid.Empty)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ"
                });
            }

            try
            {
                // Xóa tất cả phân quyền cũ của user (soft delete bằng cách set IsActive = false)
                var existingAssignments = await _context.UserBranches
                    .Where(ub => ub.UserId == request.UserId)
                    .ToListAsync();

                foreach (var assignment in existingAssignments)
                {
                    assignment.IsActive = false;
                    assignment.UpdatedDate = DateTime.Now;
                    assignment.UpdatedBy = "System"; // Có thể thay bằng user hiện tại
                }

                // Thêm phân quyền mới
                if (request.BranchIds != null && request.BranchIds.Any())
                {
                    foreach (var branchId in request.BranchIds)
                    {
                        // Kiểm tra xem đã có bản ghi chưa
                        var existingRecord = existingAssignments.FirstOrDefault(ub => ub.BranchId == branchId);
                        
                        if (existingRecord != null)
                        {
                            // Kích hoạt lại bản ghi cũ
                            existingRecord.IsActive = true;
                            existingRecord.AssignedDate = DateTime.Now;
                            existingRecord.UpdatedDate = DateTime.Now;
                            existingRecord.UpdatedBy = "System";
                        }
                        else
                        {
                            // Tạo bản ghi mới
                            var newAssignment = new UserBranch
                            {
                                Id = Guid.NewGuid(),
                                UserId = request.UserId,
                                BranchId = branchId,
                                IsActive = true,
                                AssignedDate = DateTime.Now,
                                CreatedDate = DateTime.Now,
                                CreatedBy = "System"
                            };
                            
                            _context.UserBranches.Add(newAssignment);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Phân quyền chi nhánh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lưu phân quyền: " + ex.Message
                });
            }
        }

        [HttpDelete("{userId}/branch/{branchId}")]
        public async Task<IActionResult> RemoveBranchAssignment(Guid userId, Guid branchId)
        {
            try
            {
                var assignment = await _context.UserBranches
                    .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId);

                if (assignment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy phân quyền chi nhánh"
                    });
                }

                assignment.IsActive = false;
                assignment.UpdatedDate = DateTime.Now;
                assignment.UpdatedBy = "System";

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Xóa phân quyền chi nhánh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }
    }

    public class AssignBranchRequest
    {
        public Guid UserId { get; set; }
        public List<Guid> BranchIds { get; set; } = new List<Guid>();
    }
}
