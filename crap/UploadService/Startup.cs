using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using UploadService.Configurations;
using UploadService.Connectors;
using UploadService.Extensions;
using UploadService.FeatureToggle;
using UploadService.Middlewares;
using UploadService.Middlewares.XeroContext;
using UploadService.Models;
using UploadService.Services;
using Xero.Diagnostics;

namespace UploadService
{
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
            services.AddDiagnostics();
            services.Configure<LaunchDarklyConfiguration>(Configuration.GetSection(LaunchDarklyConfiguration.Key));
            services.Configure<XeroDocsConfiguration>(Configuration.GetSection(XeroDocsConfiguration.Key));
            services.Configure<DocumentServiceConfiguration>(Configuration.GetSection(DocumentServiceConfiguration.Key));
            services.Configure<CloudStorageRouterConfiguration>(Configuration.GetSection(CloudStorageRouterConfiguration.Key));
            services.AddScoped<IXeroContextProvider, XeroContextProvider>();
            services.AddScoped<IXeroDbConnector, XeroDbConnector>();
            services.AddTransient<IDocumentServiceConnector, DocumentServiceConnector>();
            services.AddScoped<IPingable, DocumentServiceConnector>();
            services.AddScoped<CurrentUserContext>();
            services.AddScoped<IUploadService, Services.UploadService>();
            services.AddScoped<IAssociationService, AssociationService>();
            services.AddScoped<IFeatureClient, LaunchDarklyFeatureClient>();
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = GetType().Namespace });
                c.DescribeAllEnumsAsStrings();
                c.OperationFilter<SwaggerAuthoriseOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDiagnostics();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UploadService");
            });
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseGlobalException();
            app.UseMiddleware<ApiMandatoryIdentityMiddleware>();
            app.UseMvc();
            app.UseXeroContextBuilder();
        }
    }
}
