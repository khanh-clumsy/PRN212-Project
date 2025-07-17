using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class ViolationTypesDAO
    {
        //Get all violation types
        private readonly TrafficViolationFeedbackSystemContext _context;
        public ViolationTypesDAO(TrafficViolationFeedbackSystemContext context)
        {
            _context = context;
        }
        public List<ViolationType> GetAllViolationTypes()
        {
            return _context.ViolationTypes.ToList();
        }
    }
}
