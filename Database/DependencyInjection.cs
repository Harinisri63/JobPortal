using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JobPortal.Features.User.Repository;
using JobPortal.Features.User.Interface;
using JobPortal.Features.User.Candidate.Repository;
using JobPortal.Features.User.Candidate.Interface;
using JobPortal.Features.User.Admin.Repository;
using JobPortal.Features.User.Admin.Interface;
using JobPortal.Features.JobPosting;
using JobPortal.Features.JobPosting.Repository;
using JobPortal.Features.JobPosting.Interface;

namespace JobPortal.Database
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJpnsRepositories(this IServiceCollection services,IConfiguration configuration)
        {
            string connectionString =configuration.GetConnectionString("DefaultConnection")?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

            services.AddSingleton(new DatabaseConnection(connectionString));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobSeekerRepository, JobSeekerRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();

            return services;
        }
    }
}