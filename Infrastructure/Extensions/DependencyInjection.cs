using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình DbContext sử dụng PostgreSQL
            services.AddDbContext<SampleDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );
        }
    }
}
