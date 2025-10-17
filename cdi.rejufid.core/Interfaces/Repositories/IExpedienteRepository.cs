using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IExpedienteRepository
    {
        Task<IEnumerable<ExpedienteEntity>> GetAllAsync();
        Task<ExpedienteEntity> GetByIdAsync(int id);
        Task<int> CreateAsync(ExpedienteEntity expediente);
        Task<bool> DeleteAsync(int id);
        Task<(bool Existed, List<string> RutasRelativas)> DeleteDeepAsync(
            int idExpediente,
            CancellationToken ct = default
        );
        Task<IEnumerable<ExpedienteDetalleDTO>> GetLast100Async();
        Task<IEnumerable<ExpedienteDetalleDTO>> GetFilteredAsync(
            string? tipoOrgano,
            string? organo,
            string? materia,
            string? palabraClave);
    }
}