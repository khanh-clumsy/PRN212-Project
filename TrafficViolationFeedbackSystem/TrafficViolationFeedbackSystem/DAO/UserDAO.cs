using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class UserDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public UserDAO(TrafficViolationFeedbackSystemContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        /// <param name="fullName">Họ và tên</param>
        /// <param name="email">Email</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="confirmPassword">Xác nhận mật khẩu</param>
        /// <returns>Kết quả đăng ký</returns>
        public (bool success, string message) Register(string fullName, string email, string password, string confirmPassword)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    return (false, "Vui lòng điền đầy đủ thông tin bắt buộc.");
                }

                // Kiểm tra mật khẩu xác nhận
                if (password != confirmPassword)
                {
                    return (false, "Mật khẩu xác nhận không khớp.");
                }

                // Kiểm tra độ dài mật khẩu
                if (password.Length < 6)
                {
                    return (false, "Mật khẩu phải có ít nhất 6 ký tự.");
                }

                // Kiểm tra email đã tồn tại chưa
                if (_context.Users.Any(u => u.Email.ToLower() == email.ToLower()))
                {
                    return (false, "Email đã được sử dụng. Vui lòng chọn email khác.");
                }

                // Tạo user mới
                var newUser = new User
                {
                    FullName = fullName.Trim(),
                    Email = email.Trim().ToLower(),
                    Password = HashPassword(password),
                    Role = "Citizen" // Mặc định là Citizen
                };

                // Thêm vào database
                _context.Users.Add(newUser);
                _context.SaveChanges();

                return (true, "Đăng ký thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi đăng ký: {ex.Message}");
            }
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Thông tin người dùng nếu đăng nhập thành công</returns>
        public (bool success, string message, User user) Login(string email, string password)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "Vui lòng nhập email và mật khẩu.", null);
                }
                using var freshContext = new TrafficViolationFeedbackSystemContext();
                // Tìm user theo email
                var user = freshContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return (false, "Email không tồn tại trong hệ thống.", null);
                }

                // Kiểm tra mật khẩu
                if (!VerifyPassword(password.Trim(), user.Password))
                {
                    return (false, "Mật khẩu không đúng.", null);
                }

                return (true, "Đăng nhập thành công!", user);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi đăng nhập: {ex.Message}", null);
            }
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public (bool success, string message) UpdateUser(User user)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    return (false, "Không tìm thấy người dùng.");
                }

                // Cập nhật thông tin
                existingUser.FullName = user.FullName;
                existingUser.Phone = user.Phone;
                existingUser.Address = user.Address;

                _context.SaveChanges();
                return (true, "Cập nhật thông tin thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi cập nhật: {ex.Message}");
            }
        }
        public (bool success, string message) ChangePassword(int userId, string currentPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng.");
                }

                // Kiểm tra mật khẩu hiện tại
                if (!VerifyPassword(currentPassword, user.Password))
                {
                    return (false, "Mật khẩu hiện tại không đúng.");
                }

                // Kiểm tra mật khẩu mới
                if (newPassword != confirmNewPassword)
                {
                    return (false, "Mật khẩu xác nhận không khớp.");
                }

                if (newPassword.Length < 6)
                {
                    return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
                }

                // Cập nhật mật khẩu
                user.Password = HashPassword(newPassword);
                _context.SaveChanges();

                return (true, "Thay đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi thay đổi mật khẩu: {ex.Message}");
            }
        }

        public async Task<User> GetVehicleOwnerByPlateNumber(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                return null;
            }
            return await _context.Vehicles
                .Include(v => v.Owner)
                .Where(v => v.PlateNumber == plateNumber)
                .Select(v => v.Owner)
                .FirstOrDefaultAsync();
        }
        public string GetVehicleOwnerNameByPlateNumber(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                return null;
            }
            return _context.Vehicles
                .Include(v => v.Owner)
                .Where(v => v.PlateNumber == plateNumber)
                .Select(v => v.Owner.FullName)
                .FirstOrDefault();
        }
        public List<Vehicle> GetVehiclesByOwnerId(int ownerId)
        {
            return _context.Vehicles
                .Where(v => v.OwnerId == ownerId)
                .ToList();
        }



    }
}
