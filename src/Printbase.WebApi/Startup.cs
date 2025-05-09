using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Printbase.Application.Products.Commands.CreateProduct;
using Printbase.Domain.Repositories;
using Printbase.Infrastructure.Database;
using Printbase.Infrastructure.Mappings;
using Printbase.Infrastructure.Repositories;

namespace Printbase.WebApi;

public static class Startup
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
        services.AddScoped<IProductGroupRepository, ProductGroupRepository>();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        }, typeof(ProductMappingProfile).Assembly);
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddOpenApi();
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (app is WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                dbContext.Database.Migrate();
                Console.WriteLine("Database migrations applied successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
            }
        }
        
        if (env.IsDevelopment())
        {
            Console.WriteLine("Development environment variables applied");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            Console.WriteLine("Production environment variables applied");
            app.UseExceptionHandler("/Error");
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}