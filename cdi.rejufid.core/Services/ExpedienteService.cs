using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.core.Interfaces.Services;

namespace cdi.rejufid.core.Services
{
    public class ExpedienteService : IExpedienteService
    {
        private readonly IExpedienteRepository expedienteRepository;
        private readonly IDocumentoService documentoService;
        private readonly IMapper mapper;

        public ExpedienteService(IExpedienteRepository expedienteRepository, IDocumentoService documentoService, IMapper mapper)
        {
            this.expedienteRepository = expedienteRepository;
            this.documentoService = documentoService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ExpedienteDTO>> GetAllAsync()
        {
            var data = await expedienteRepository.GetAllAsync();
            return mapper.Map<IEnumerable<ExpedienteDTO>>(data);
        }

        public async Task<ExpedienteDTO> GetByIdAsync(int id)
        {
            var data = await expedienteRepository.GetByIdAsync(id);
            return mapper.Map<ExpedienteDTO>(data);
        }

        public async Task<int> CreateAsync(ExpedienteDTO dto)
        {
            var entity = mapper.Map<ExpedienteEntity>(dto);
            return await expedienteRepository.CreateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await documentoService.DeleteByExpedienteAsync(id);
            return await expedienteRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ExpedienteDetalleDTO>> GetFilteredAsync(
            string? tipoOrgano,
            string? organo,
            string? materia,
            string? palabraClave)
        {
            var data = await expedienteRepository.GetFilteredAsync(
                tipoOrgano, organo, materia, palabraClave);

            return data;
        }

        public async Task<IEnumerable<ExpedienteDetalleDTO>> GetLast100Async()
        {
            var data = await expedienteRepository.GetLast100Async();
            return data;
        }

        public async Task<bool> DeleteDeepAsync(
            int idExpediente,
            string baseUploadsAbsolutePath,
            CancellationToken ct = default)
        {
            var (existed, rutasRelativas) = await expedienteRepository.DeleteDeepAsync(idExpediente, ct);
            if (!existed) return false;

            foreach (var rel in rutasRelativas.Distinct())
            {
                var abs = ToAbsolutePath(baseUploadsAbsolutePath, rel);
                try
                {
                    if (!string.IsNullOrWhiteSpace(abs) && File.Exists(abs))
                        File.Delete(abs);
                }
                catch
                {
                    
                }
            }

            return true;
        }

        private static string? ToAbsolutePath(string baseUploadsAbsolutePath, string? ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta)) return null;

            if (Path.IsPathRooted(ruta)) return ruta;

            var trimmed = ruta.TrimStart('/', '\\')
                              .Replace('/', Path.DirectorySeparatorChar)
                              .Replace('\\', Path.DirectorySeparatorChar);

            if (trimmed.StartsWith("archivos" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(("archivos" + Path.DirectorySeparatorChar).Length);
            }

            return Path.Combine(baseUploadsAbsolutePath, trimmed);
        }
    }
}
