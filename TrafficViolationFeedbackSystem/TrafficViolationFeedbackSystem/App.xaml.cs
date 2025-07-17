using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Views.Citizen;

namespace TrafficViolationFeedbackSystem
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Load cấu hình từ appsettings.Local.json
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Local.json", optional: false)
                .Build();

            // Đăng ký config để lấy connection string
            services.AddSingleton(configuration);

            // Đăng ký DbContext
            services.AddDbContext<TrafficViolationFeedbackSystemContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Đăng ký DAO
            services.AddScoped<UserDAO>();
            // AddScoped<ReportDAO>() v.v nếu cần

            Services = services.BuildServiceProvider();

            // Kiểm tra kết nối DB
            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TrafficViolationFeedbackSystemContext>();
                db.Database.OpenConnection();
                db.Database.CloseConnection();

                MessageBox.Show("✅ [INFO] Kết nối CSDL thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Không thể kết nối CSDL: {ex.Message}");
                Shutdown();
            }

            //Khởi động UI
            var loginWindow = new Views.Authentication.LoginWindow();
            loginWindow.Show();

            //var cizitenDashboardWindow = new Views.Citizen.CitizenDashboardWindow();
            //cizitenDashboardWindow.Show();
        }
    }
}
