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
        private readonly IMapper mapper;

        public ExpedienteService(IExpedienteRepository expedienteRepository, IMapper mapper)
        {
            this.expedienteRepository = expedienteRepository;
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
            return await expedienteRepository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, ExpedienteDTO dto)
        {
            var entity = mapper.Map<ExpedienteEntity>(dto);
            return await expedienteRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await expedienteRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ExpedienteDetalleDTO>> GetFilteredAsync(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? materia,
            string? estado,
            string? tipoOrgano,
            string? palabraClave)
        {
            var data = await expedienteRepository.FiltrarDetallesAsync(
                fechaDesde, fechaHasta, materia, estado, tipoOrgano, palabraClave);
            return data;
        }

        public async Task<IEnumerable<ExpedienteDetalleDTO>> GetUltimos100Async()
        {
            var data = await expedienteRepository.FiltrarUltimos100Async();
            return data;
        }
    }
}
