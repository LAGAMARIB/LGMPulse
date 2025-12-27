using LGMDAL;
using LGMPulse.Domain;
using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Entities;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class MovtoRepository : BaseRepository<Movto, MovtoEntity>, IMovtoRepository
{
    public async Task<SumarioMes?> GetSumario(DateTime dataIni, DateTime dataFim)
    {
        using (var ctx = NewDBContext())
        {
            DBStoredProcedure sp = ctx.GetStoredProcedure("sp_GetTotaisMovto");
            sp.AddParameter("p_dbKey", ctx.DBKey);
            sp.AddParameter("p_DataInicial", dataIni.ToString("yyyy-MM-dd"));
            sp.AddParameter("p_DataFinal", dataFim.ToString("yyyy-MM-dd"));
            var result = await sp.ExecuteReaderAsync(reader => new SumarioMes
            {
                TotalReceitas = reader.GetDecimal("TotalReceitas"),
                TotalDespesas = reader.GetDecimal("TotalDespesas")
            });
            return result;
        }
    }
}
