using cdi.rejufid.core.DTOs;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface IExpedienteService
    {
        Task<IEnumerable<ExpedienteDTO>> GetAllAsync();
        Task<ExpedienteDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(ExpedienteDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteDeepAsync(int idExpediente, string baseUploadsAbsolutePath, CancellationToken ct = default);
        Task<IEnumerable<ExpedienteDetalleDTO>> GetFilteredAsync(
            string? tipoOrgano,
            string? organo,
            string? materia,
            string? palabraClave);
        Task<IEnumerable<ExpedienteDetalleDTO>> GetLast100Async();
    }
}
