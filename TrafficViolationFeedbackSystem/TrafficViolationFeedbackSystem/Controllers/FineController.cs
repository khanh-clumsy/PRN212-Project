using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Services;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Controllers
{
    public class FineController
    {
        private readonly FineDAO _fineDAO;

        public FineController()
        {
            _fineDAO = new FineDAO();
        }

        public List<FineViewModel> LoadPendingFinesForCurrentUser()
        {
            int currentUserId = int.Parse(AuthenticationContext.UserId);
            return _fineDAO.GetPendingFinesByUserId(currentUserId);
        }

        public List<FineViewModel> GetAllByCurrentUser()
        {
            int currentUserId = int.Parse(AuthenticationContext.UserId);
            return _fineDAO.GetAll(currentUserId);
        }
    }

}
