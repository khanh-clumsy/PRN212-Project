using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class ViewAllViolationsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public ViewAllViolationsView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAllViolations();
            LoadViolationTypes();
            LoadStatuses();
        }

        private void LoadAllViolations()
        {
            ViolationDAO dao = new ViolationDAO();
            var violations = dao.GetAllViolations();
            dgAllViolations.ItemsSource = violations;
        }

        private void LoadViolationTypes()
        {
            var types = _context.ViolationTypes.ToList();
            ViolationTypeFilterBox.ItemsSource = types;
            ViolationTypeFilterBox.DisplayMemberPath = "Name";
            ViolationTypeFilterBox.SelectedValuePath = "ViolationTypeId";
        }

        private void LoadStatuses()
        {
            var statuses = new List<string>
                {
                    "Active",
                    "Appealed",
                    "RejectedAppeal",
                    "Cancelled",
                    "Paid"
                };
            StatusFilterBox.ItemsSource = statuses;
        }



        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var keyword = SearchBox.Text.Trim();
            ViolationDAO dao = new ViolationDAO();
            var results = dao.SearchByDescription(keyword);
            dgAllViolations.ItemsSource = results;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            int? typeId = null;
            string status = null;

            if (ViolationTypeFilterBox.SelectedItem is ViolationType selectedType)
                typeId = selectedType.ViolationTypeId;

            if (StatusFilterBox.SelectedItem is string statusStr)
                status = statusStr;

            ViolationDAO dao = new ViolationDAO();
            var results = dao.FilterByTypeAndStatus(typeId, status);
            dgAllViolations.ItemsSource = results;
        }
    }
}
