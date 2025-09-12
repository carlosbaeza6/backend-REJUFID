using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Reflection;
using cdi.rejufid.infrastructure.Extensions;
using cdi.rejufid.infrastructure.Mappings;

namespace cdi.rejufid.api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // CONFIGURACIÓN DE SERVICIOS PRINCIPALES
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    policyBuilder => policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true));
            });

            // Conexión a la Base de Datos 
            services.AddDbContext(Configuration)
                    .AddFilters()
                    .AddServices();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Autorización
            services.AddAuthorization(Configuration);

            // CONFIGURACIÓN DE SWAGGER
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "REJUFID Api", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Busca el método Usuario/Login, autentícate y pega el token: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Incluir XML solo si existe
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ENTORNO Y DOCUMENTACIÓN
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "REJUFID Api");
                options.RoutePrefix = "swagger";
            });

            // SEGURIDAD Y RUTEO
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // ARCHIVOS ESTÁTICOS
            var archivosFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "archivos");
            if (!Directory.Exists(archivosFolderPath))
            {
                Directory.CreateDirectory(archivosFolderPath);
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(archivosFolderPath),
                RequestPath = new PathString("/archivos")
            });
        }
    }
}

