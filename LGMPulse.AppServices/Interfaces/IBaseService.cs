using LGMDomains.Common;

namespace LGMPulse.AppServices.Interfaces;

public interface IBaseService<TDomain> where TDomain : BaseDomain, new()
{
    Task<LGMResult<TDomain>> GetByIdAsync(int id);
    Task<LGMResult<List<TDomain>>> GetListAsync(TDomain? filterIni = null, TDomain? filterFim = null, string? sortBy = null, List<string>? fields = null);
    Task<ILGMResult> CreateAsync(TDomain domain);
    Task<ILGMResult> UpdateAsync(TDomain domain, List<string>? campos = null);
    Task<ILGMResult> DeleteAsync(int? id);

    Task<LGMResult<TDomain>> GetFirstAsync(TDomain objSelecIni,
                                 TDomain objSelecFim,
                                 string? pSort = null,
                                 List<string>? fields = null);

    Task<LGMResult<TDomain>> GetFirstAsync(TDomain objSelecIni,
                                 string? pSort = null,
                                 List<string>? fields = null);

}