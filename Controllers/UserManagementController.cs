using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_trial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IdeaBoardDbContext _dbContext;

        public UserManagementController(IdeaBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all users
        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _dbContext.Users
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role.ToString(),
                        Status = u.Status.ToString()
                    })
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving users", Error = ex.Message });
            }
        }

        // Get users by role
        [HttpGet("users/role/{role}")]
        public async Task<ActionResult> GetUsersByRole(string role)
        {
            try
            {
                // Validate role
                if (!Enum.TryParse<UserRole>(role, true, out var userRole))
                {
                    return BadRequest(new { Message = "Invalid role. Valid values are: Employee, Manager, Admin" });
                }

                var users = await _dbContext.Users
                    .Where(u => u.Role == userRole)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role.ToString(),
                        Status = u.Status.ToString()
                    })
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving users by role", Error = ex.Message });
            }
        }

        // Get users by status
        [HttpGet("users/status/{status}")]
        public async Task<ActionResult> GetUsersByStatus(string status)
        {
            try
            {
                // Validate status
                if (!Enum.TryParse<UserStatus>(status, true, out var userStatus))
                {
                    return BadRequest(new { Message = "Invalid status. Valid values are: Active, Inactive" });
                }

                var users = await _dbContext.Users
                    .Where(u => u.Status == userStatus)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role.ToString(),
                        Status = u.Status.ToString()
                    })
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving users by status", Error = ex.Message });
            }
        }

        // Get user by ID with detailed information
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _dbContext.Users
                    .Where(u => u.UserId == userId)
                    .Include(u => u.SubmittedIdeas)
                    .Include(u => u.Comments)
                    .Include(u => u.Votes)
                    .Include(u => u.ReviewsAuthored)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var userDetail = new UserDetailResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    IdeasSubmitted = user.SubmittedIdeas.Count,
                    CommentsPosted = user.Comments.Count,
                    VotesCasted = user.Votes.Count,
                    ReviewsSubmitted = user.ReviewsAuthored.Count
                };

                return Ok(userDetail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving user", Error = ex.Message });
            }
        }

        // Get user by email
        [HttpGet("email/{email}")]
        public async Task<ActionResult> GetUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { Message = "Email cannot be empty" });
                }

                var user = await _dbContext.Users
                    .Where(u => u.Email.ToLower() == email.ToLower())
                    .Include(u => u.SubmittedIdeas)
                    .Include(u => u.Comments)
                    .Include(u => u.Votes)
                    .Include(u => u.ReviewsAuthored)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var userDetail = new UserDetailResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    IdeasSubmitted = user.SubmittedIdeas.Count,
                    CommentsPosted = user.Comments.Count,
                    VotesCasted = user.Votes.Count,
                    ReviewsSubmitted = user.ReviewsAuthored.Count
                };

                return Ok(userDetail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving user by email", Error = ex.Message });
            }
        }

        // Toggle user status (Active/Inactive)
        [HttpPut("{userId}/status")]
        public async Task<ActionResult> ToggleUserStatus(Guid userId, [FromBody] ToggleUserStatusRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate status
                if (!Enum.TryParse<UserStatus>(request.Status, true, out var newStatus))
                {
                    return BadRequest(new { Message = "Invalid status. Valid values are: Active, Inactive" });
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                // Prevent deactivating the current admin user
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == userId.ToString() && newStatus == UserStatus.Inactive)
                {
                    return BadRequest(new { Message = "You cannot deactivate your own account" });
                }

                var oldStatus = user.Status;
                user.Status = newStatus;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString()
                };

                return Ok(new { Message = $"User status changed from {oldStatus} to {newStatus}", User = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error toggling user status", Error = ex.Message });
            }
        }

        // Activate user
        [HttpPut("{userId}/activate")]
        public async Task<ActionResult> ActivateUser(Guid userId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                if (user.Status == UserStatus.Active)
                {
                    return BadRequest(new { Message = "User is already active" });
                }

                user.Status = UserStatus.Active;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString()
                };

                return Ok(new { Message = "User activated successfully", User = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error activating user", Error = ex.Message });
            }
        }

        // Deactivate user
        [HttpPut("{userId}/deactivate")]
        public async Task<ActionResult> DeactivateUser(Guid userId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                // Prevent deactivating the current admin user
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == userId.ToString())
                {
                    return BadRequest(new { Message = "You cannot deactivate your own account" });
                }

                if (user.Status == UserStatus.Inactive)
                {
                    return BadRequest(new { Message = "User is already inactive" });
                }

                user.Status = UserStatus.Inactive;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString()
                };

                return Ok(new { Message = "User deactivated successfully", User = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deactivating user", Error = ex.Message });
            }
        }

        // Update user role
        [HttpPut("{userId}/role")]
        public async Task<ActionResult> UpdateUserRole(Guid userId, [FromBody] UpdateUserRoleRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate role
                if (!Enum.TryParse<UserRole>(request.Role, true, out var newRole))
                {
                    return BadRequest(new { Message = "Invalid role. Valid values are: Employee, Manager, Admin" });
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                // Prevent changing the current admin user's role
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == userId.ToString())
                {
                    return BadRequest(new { Message = "You cannot change your own role" });
                }

                var oldRole = user.Role;
                user.Role = newRole;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString()
                };

                return Ok(new { Message = $"User role changed from {oldRole} to {newRole}", User = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating user role", Error = ex.Message });
            }
        }

        // Get statistics of users
        [HttpGet("statistics/summary")]
        public async Task<ActionResult> GetUserStatistics()
        {
            try
            {
                var totalUsers = await _dbContext.Users.CountAsync();
                var activeUsers = await _dbContext.Users.CountAsync(u => u.Status == UserStatus.Active);
                var inactiveUsers = await _dbContext.Users.CountAsync(u => u.Status == UserStatus.Inactive);

                var employeeCount = await _dbContext.Users.CountAsync(u => u.Role == UserRole.Employee);
                var managerCount = await _dbContext.Users.CountAsync(u => u.Role == UserRole.Manager);
                var adminCount = await _dbContext.Users.CountAsync(u => u.Role == UserRole.Admin);

                var statistics = new
                {
                    totalUsers,
                    activeUsers,
                    inactiveUsers,
                    roleBreakdown = new
                    {
                        employees = employeeCount,
                        managers = managerCount,
                        admins = adminCount
                    }
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving user statistics", Error = ex.Message });
            }
        }

        // Search users by name
        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult> SearchUsers(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { Message = "Search term cannot be empty" });
                }

                var users = await _dbContext.Users
                    .Where(u => u.Name.ToLower().Contains(searchTerm.ToLower()) || 
                                u.Email.ToLower().Contains(searchTerm.ToLower()))
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role.ToString(),
                        Status = u.Status.ToString()
                    })
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                if (!users.Any())
                {
                    return NotFound(new { Message = "No users found matching the search criteria" });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error searching users", Error = ex.Message });
            }
        }
    }
}
