using LGMDAL;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.ViewModels;
using LGMPulse.Persistence.Entities;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class MovtoRepository : BaseRepository<Movto, MovtoEntity>, IMovtoRepository
{
    public async Task<List<SumarioMes>> GetSumario(DateTime dataIni, DateTime dataFim)
    {
        using (var ctx = NewDBContext())
        {
            string sql = @$"SELECT 
                            IFNULL(SUM(CASE WHEN TipoMovto = 0 THEN ValorMovto ELSE 0 END), 0) AS TotalReceitas, 
                            IFNULL(SUM(CASE WHEN TipoMovto = 1 THEN ValorMovto ELSE 0 END), 0) AS TotalDespesas 
                            FROM {ctx.DBKey}_movto 
                            WHERE DataMovto >= '{dataIni.ToString("yyyy-MM-dd")}' 
                              AND DataMovto <= '{dataFim.ToString("yyyy-MM-dd")}'";
            var result = await ctx.GetListAsync<SumarioMes>(sql, reader => new()
            {
                TotalReceitas = reader.GetDecimal("TotalReceitas"),
                TotalDespesas = reader.GetDecimal("TotalDespesas")
            });
            return result;
        }

        //using (var ctx = NewDBContext())
        //{
        //    DBStoredProcedure sp = ctx.GetStoredProcedure("sp_GetTotaisMovto");
        //    sp.AddParameter("p_dbKey", ctx.DBKey);
        //    sp.AddParameter("p_DataInicial", dataIni.ToString("yyyy-MM-dd"));
        //    sp.AddParameter("p_DataFinal", dataFim.ToString("yyyy-MM-dd"));
        //    var result = await sp.ExecuteReaderAsync(reader => new SumarioMes
        //    {
        //        TotalReceitas = reader.GetDecimal("TotalReceitas"),
        //        TotalDespesas = reader.GetDecimal("TotalDespesas")
        //    });
        //    return result;
        //}
    }

}

