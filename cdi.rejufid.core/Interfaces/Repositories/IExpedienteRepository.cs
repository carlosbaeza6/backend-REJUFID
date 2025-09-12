using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IExpedienteRepository
    {
        Task<IEnumerable<ExpedienteEntity>> GetAllAsync();
        Task<ExpedienteEntity> GetByIdAsync(int id);
        Task<int> AddAsync(ExpedienteEntity expediente);
        Task<bool> UpdateAsync(ExpedienteEntity expediente);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ExpedienteDetalleDTO>> FiltrarUltimos100Async();
        Task<IEnumerable<ExpedienteDetalleDTO>> FiltrarDetallesAsync(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? materia,
            string? estado,
            string? tipoOrgano,
            string? palabraClave);
    }
}