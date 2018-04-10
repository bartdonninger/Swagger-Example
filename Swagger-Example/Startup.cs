using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using NSwag.SwaggerGeneration.Processors.Security;

namespace Swagger_Example
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:60109").AllowAnyMethod().AllowAnyHeader());
            });
            services.AddMvc();
            services.AddSwagger();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("AllowSpecificOrigin");

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable the Swagger UI middleware and the Swagger generator
            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
                settings.GeneratorSettings.Title = "Test Swagger API";
                settings.GeneratorSettings.Title = "Test Swagger Description";
                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("apikey",
                    new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "api_key",
                        In = SwaggerSecurityApiKeyLocation.Header,
                        
                    }));
                settings.GeneratorSettings.DocumentProcessors.Add(new MyDocumentProcessor());
            });

            app.UseMvc();
        }

        public class MyDocumentProcessor : IDocumentProcessor
        {

            // TODO: This is not included in the header :(
            public async Task ProcessAsync(DocumentProcessorContext context)
            {
                context.Document.Parameters.Add("test", new SwaggerParameter
                    {
                        Name = "TestHeader",
                        Type = JsonObjectType.String,
                        Kind = SwaggerParameterKind.Header,
                        IsRequired = true,
                        Description = "blaa description"
                    }
                );
            }
        }
    }
}
