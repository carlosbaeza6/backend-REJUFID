using cdi.rejufid.core.DTOs;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface IExpedienteService
    {
        Task<IEnumerable<ExpedienteDTO>> GetAllAsync();
        Task<ExpedienteDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(ExpedienteDTO dto);
        Task<bool> UpdateAsync(int id, ExpedienteDTO dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<ExpedienteDetalleDTO>> GetFilteredAsync(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? materia,
            string? estado,
            string? tipoOrgano,
            string? palabraClave);
        Task<IEnumerable<ExpedienteDetalleDTO>> GetUltimos100Async();
    }
}
