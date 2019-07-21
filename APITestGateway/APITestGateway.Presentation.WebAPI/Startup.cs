using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using APITestGateway.Presentation.WebAPI.SwaggerDocs.Attributes;
using APITestGateway.Presentation.WebAPI.Models;
using APITestGateway.Presentation.WebAPI.SwaggerDocs.Filters;

namespace APITestGateway.Presentation.WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly SymmetricSecurityKey _signingKey;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWTOptions:Secret"]));
        }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JWTOptions>(x => Configuration.Bind("JWTOptions", x));
            services.Configure<Routes>(x => Configuration.Bind("Routes", x));
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();
            services.AddMvc()
                .AddJsonOptions(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["JWTOptions:Issuer"],
                    ValidAudience = Configuration["JWTOptions:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,

                    ValidateLifetime = true
                };
            });

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "APITESTGATEWAY", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Inform the JWT token with Bearer", Name = "Authorization", Type = "apiKey" });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "APITESTGATEWAY.xml");
                c.IncludeXmlComments(filePath);
                c.OperationFilter<ExamplesOperationFilter>();
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.ConfigureSwaggerGen(x =>
            {
                x.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                x.OperationFilter<ProducesOperatioFilter>();
            });
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"../swagger/v1/swagger.json", "APITESTGATEWAY");
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
