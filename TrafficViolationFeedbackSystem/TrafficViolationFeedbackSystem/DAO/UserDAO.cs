using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class UserDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public UserDAO(TrafficViolationFeedbackSystemContext _context)
        {
            this._context = _context;
        }
        public void Register(string name, string email, string password, string confirmPassword)
        {
           
        }



    }
}
