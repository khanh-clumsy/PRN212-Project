using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Controllers
{
    public class UserController
    {
        private readonly UserDAO _userDAO;
        private readonly TrafficViolationFeedbackSystemContext _context;
        public UserController(TrafficViolationFeedbackSystemContext context)
        {
            _context = context;
            _userDAO = new UserDAO(_context);
        }

        public (bool success, string message) Register(string fullName, string email, string password, string confirmPassword)
        {
            return _userDAO.Register(fullName, email, password, confirmPassword);
        }

        public (bool success, string message, User user) Login(string email, string password)
        {
            return _userDAO.Login(email, password);
        }

        public User GetUserById(int userId)
        {
            return _userDAO.GetUserById(userId);
        }

        public User GetUserByEmail(string email)
        {
            return _userDAO.GetUserByEmail(email);
        }
        public (bool success, string message) UpdateUser(User user)
        {
            return _userDAO.UpdateUser(user);
        }
        public (bool success, string message) ChangePassword(int userId, string currentPassword, string newPassword, string confirmNewPassword)
        {
            return _userDAO.ChangePassword(userId, currentPassword, newPassword, confirmNewPassword);
        }

        public bool IsEmailExists(string email)
        {
            var user = _userDAO.GetUserByEmail(email);
            return user != null;
        }

        public bool ValidateLoginCredentials(string email, string password)
        {
            var user = _userDAO.GetUserByEmail(email);
            if (user == null) return false;

            var (success, _, _) = _userDAO.Login(email, password);
            return success;
        }

        public async Task<User> GetVehicleOwnerByPlateNumber(string plateNumber)
        {
            return await _userDAO.GetVehicleOwnerByPlateNumber(plateNumber);
        }
    }
}
