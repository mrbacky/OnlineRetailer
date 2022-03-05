﻿using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi;

public class Startup
{
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }
    
            public IConfiguration Configuration { get; }
    
            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                // In-memory database:
                services.AddDbContext<CustomerApiContext>(opt => opt.UseInMemoryDatabase("CustomersDb"));
    
                // Register repositories for dependency injection
                services.AddScoped<IRepository<Customer>, CustomerRepository>();
    
                // Register database initializer for dependency injection
                services.AddTransient<IDbInitializer, DbInitializer>();
    
                services.AddControllers();
            }
    
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                // Initialize the database
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    // Initialize the database
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetService<CustomerApiContext>();
                    var dbInitializer = services.GetService<IDbInitializer>();
                    dbInitializer.Initialize(dbContext);
                }
    
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
    
                // app.UseHttpsRedirection();
    
                app.UseRouting();
    
                app.UseAuthorization();
    
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
    
}