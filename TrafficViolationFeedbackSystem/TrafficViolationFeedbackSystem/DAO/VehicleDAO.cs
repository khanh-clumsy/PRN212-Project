using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class VehicleDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public VehicleDAO()
        {
            _context = new TrafficViolationFeedbackSystemContext();
        }

        public List<Vehicle> GetAllVehiclesWithUsers()
        {
            return _context.Vehicles
                           .Include(v => v.Owner)
                           .ToList();
        }
        public List<Vehicle> SearchVehicles(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                return GetAllVehiclesWithUsers();
            }

            plateNumber = plateNumber.ToLower();

            return _context.Vehicles
                           .Include(v => v.Owner)
                           .Where(v => v.PlateNumber.ToLower().Contains(plateNumber))
                           .ToList();
        }
        public void DeleteVehicle(int vehicleId)
        {
            var vehicle = _context.Vehicles.Find(vehicleId);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
            }
        }

    }
}
