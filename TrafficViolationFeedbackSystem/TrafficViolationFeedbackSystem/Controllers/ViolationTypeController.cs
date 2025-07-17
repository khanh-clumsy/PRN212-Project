using System.Collections.Generic;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Controllers
{
    public class ViolationTypeController
    {
        private readonly ViolationTypesDAO _violationTypesDAO;

        public ViolationTypeController(TrafficViolationFeedbackSystemContext context)
        {
            _violationTypesDAO = new ViolationTypesDAO(context);
        }

        public List<ViolationType> GetAll()
        {
            return _violationTypesDAO.GetAllViolationTypes();
        }
    }
}
