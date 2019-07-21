using System.IO;
using APITestRegister.Infra.CrossCutting;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using APITestRegister.Presentation.WebAPI.SwaggerDocs.Attributes;
using APITestRegister.Domain.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using APITestRegister.Presentation.WebAPI.SwaggerDocs.Filters;

namespace APITestRegister.Presentation.WebAPI
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
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();
            services.AddMvc()
                .AddJsonOptions(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            services.AddAutoMapper();
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

            var conStrings = new ConnectionStrings();
            Configuration.Bind("ConnectionStrings", conStrings);
            services.AddSingleton(conStrings);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "APITESTREGISTER", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Inform the JWT token with Bearer", Name = "Authorization", Type = "apiKey" });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "APITESTREGISTER.xml");
                c.IncludeXmlComments(filePath);
                c.OperationFilter<ExamplesOperationFilter>();
            });

            services.ConfigureSwaggerGen(x =>
            {
                x.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                x.OperationFilter<ProducesOperatioFilter>();
            });

            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));
            NativeInjectorBootStrapper.RegisterServices(services, Configuration);
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
                c.SwaggerEndpoint($"../swagger/v1/swagger.json", "APITESTREGISTER");
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
