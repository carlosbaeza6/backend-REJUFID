using cdi.core;
using cdi.core.Filters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.infrastructure.Repositories;
using cdi.rejufid.core.Interfaces;
using cdi.rejufid.core.Interfaces.Services;
using cdi.rejufid.core.Services;

namespace cdi.rejufid.infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connections = new Dictionary<DBConnectionsNames, string>
            {
                { DBConnectionsNames.REJUFIDDB, configuration.GetConnectionString("REJUFIDDB") }
            };

            services.AddSingleton<IDictionary<DBConnectionsNames, string>>(connections);
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Repositorios y Unit of Work
            services.AddTransient<IExpedienteRepository, ExpedienteRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Servicios necesarios para Expedientes
            services.AddTransient<IExpedienteService, ExpedienteService>();
            services.AddTransient<ICatalogosService, CatalogosService>();
            // Servicios necesarios para Catálogos
            services.AddTransient<IEstadoRepository, EstadoRepository>();
            services.AddTransient<IEstatusRepository, EstatusRepository>();
            services.AddTransient<IMateriaRepository, MateriaRepository>();
            services.AddTransient<IOrganoRepository, OrganoRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<ITipoAsuntoRepository, TipoAsuntoRepository>();
            services.AddTransient<ITipoOrganoRepository, TipoOrganoRepository>();
            services.AddTransient<ITipoArchivoRepository, TipoArchivoRepository>();


            // Servicios adicionales (comentados por ahora)
            // services.AddTransient<IDocumentoService, DocumentoService>();
            // services.AddTransient<ITipoArchivoService, TipoArchivoService>();
            // services.AddTransient<IUsuarioRolService, UsuarioRolService>();
            // services.AddTransient<IAccionUsuarioService, AccionUsuarioService>();

            return services;
        }

        public static IServiceCollection AddFilters(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
                    .AddFluentValidationClientsideAdapters()
                    .AddMvc();

            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            return services;
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration Configuration)
        {
            var key = Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SecretKey"));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
