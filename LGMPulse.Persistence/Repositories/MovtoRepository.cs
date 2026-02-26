using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.Domain.ViewModels;
using LGMPulse.Persistence.Entities;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class MovtoRepository : BaseRepository<Movto, MovtoEntity>, IMovtoRepository
{
    public async Task<SumarioMes?> GetSumarioMes(DateTime dataIni, DateTime dataFim)
    {
        dataFim = dataFim.AddDays(1).Date;
        using (var ctx = NewDBContext())
        {
            string sql = @$"SELECT 
                            IFNULL(SUM(CASE WHEN TipoMovto = 0 THEN ValorMovto ELSE 0 END), 0) AS TotalReceitas, 
                            IFNULL(SUM(CASE WHEN TipoMovto = 1 THEN ValorMovto ELSE 0 END), 0) AS TotalDespesas 
                            FROM {ctx.DBKey}_movto 
                            WHERE DataMovto >= '{dataIni.ToString("yyyy-MM-dd")}' 
                              AND DataMovto < '{dataFim.ToString("yyyy-MM-dd")}'";
            var result = await ctx.GetListAsync<SumarioMes>(sql, reader => new()
            {
                TotalReceitas = reader.GetDecimal("TotalReceitas"),
                TotalDespesas = reader.GetDecimal("TotalDespesas")
            });
            SumarioMes? sumario = result.FirstOrDefault();
            return sumario;
        }
    }

    public async Task<List<SumarioPeriodo>> GetSumarioPeriodo(DateTime dataIni, DateTime dataFim)
    {
        dataFim = dataFim.AddDays(1).Date;
        using (var ctx = NewDBContext())
        {
            string sql = $@"SELECT
                                YEAR(DataMovto)  AS Ano,
                                MONTH(DataMovto) AS Mes,

                                IFNULL(SUM(CASE 
                                    WHEN TipoMovto = 0 THEN ValorMovto 
                                    ELSE 0 
                                END), 0) AS TotalReceitas,

                                IFNULL(SUM(CASE 
                                    WHEN TipoMovto = 1 THEN ValorMovto 
                                    ELSE 0 
                                END), 0) AS TotalDespesas

                            FROM {ctx.DBKey}_movto
                            WHERE DataMovto IS NOT NULL
                              AND DataMovto >= '{dataIni.ToString("yyyy-MM-dd")}'
                              AND DataMovto < '{dataFim.ToString("yyyy-MM-dd")}'
                            GROUP BY
                                YEAR(DataMovto),
                                MONTH(DataMovto)
                            ORDER BY
                                Ano,
                                Mes;
                            ";

            var sumarios = await ctx.GetListAsync<SumarioPeriodo>(sql, reader => new SumarioPeriodo
            {
                Ano = reader.GetInt32("Ano"),
                Mes = reader.GetInt32("Mes"),
                TotalReceitas = reader.GetDecimal("TotalReceitas"),
                TotalDespesas = reader.GetDecimal("TotalDespesas")
            });
            return sumarios;
        }
    }

    public async Task<List<GrupoSumary>> GetListGrupoSumary(DateTime dataIni, DateTime dataFim)
    {
        dataFim = dataFim.AddDays(1).Date;
        using (var ctx = NewDBContext())
        {
            string sql = $@"SELECT
                                g.ID            AS IDGrupo,
                                g.Descricao     AS DescGrupo,
                                g.TipoMovto     AS TipoMovto,
                                SUM(m.ValorMovto) AS ValorGrupo
                            FROM {ctx.DBKey}_movto m
                            INNER JOIN {ctx.DBKey}_grupo g
                                    ON g.ID = m.IDGrupo
                            WHERE m.DataMovto >= '{dataIni.ToString("yyyy-MM-dd")}'
                              AND m.DataMovto <  '{dataFim.ToString("yyyy-MM-dd")}'
                            GROUP BY
                                g.ID,
                                g.Descricao,
                                g.TipoMovto
                            ORDER BY
                                g.TipoMovto,
                                ValorGrupo DESC;";

            var grupos = await ctx.GetListAsync<GrupoSumary>(sql, reader => new GrupoSumary
            {
                IDGrupo = reader.GetInt32("IDGrupo"),
                DescGrupo = reader.GetString("DescGrupo"),
                TipoMovto = (TipoMovtoEnum)reader.GetInt32("TipoMovto"),
                ValorGrupo = reader.GetDecimal("ValorGrupo")
            });
            return grupos;
        }
    }

    public async Task<List<MapaModel>> GetMapaFinanceiroAsync(int year)
    {
        DateTime dataIni = new DateTime(year, 1, 1);
        DateTime dataFim = new DateTime(year + 1, 1, 1);
        using (var ctx = NewDBContext())
        {
            string sql = $@"SELECT
                                g.ID                AS IDGrupo,
                                g.TipoMovto         AS TipoMovto,
                                g.Descricao         AS DescGrupo,

                                SUM(CASE WHEN MONTH(m.DataMovto) = 1  THEN m.ValorMovto ELSE 0 END) AS Mes01,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 2  THEN m.ValorMovto ELSE 0 END) AS Mes02,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 3  THEN m.ValorMovto ELSE 0 END) AS Mes03,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 4  THEN m.ValorMovto ELSE 0 END) AS Mes04,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 5  THEN m.ValorMovto ELSE 0 END) AS Mes05,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 6  THEN m.ValorMovto ELSE 0 END) AS Mes06,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 7  THEN m.ValorMovto ELSE 0 END) AS Mes07,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 8  THEN m.ValorMovto ELSE 0 END) AS Mes08,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 9  THEN m.ValorMovto ELSE 0 END) AS Mes09,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 10 THEN m.ValorMovto ELSE 0 END) AS Mes10,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 11 THEN m.ValorMovto ELSE 0 END) AS Mes11,
                                SUM(CASE WHEN MONTH(m.DataMovto) = 12 THEN m.ValorMovto ELSE 0 END) AS Mes12

                            FROM {ctx.DBKey}_grupo g
                            LEFT JOIN {ctx.DBKey}_movto m
                                   ON m.IDGrupo = g.ID
                                  AND m.DataMovto >= '{dataIni.ToString("yyyy-MM-dd")}'
                                  AND m.DataMovto <  '{dataFim.ToString("yyyy-MM-dd")}'

                            GROUP BY
                                g.ID,
                                g.TipoMovto,
                                g.Descricao

                            ORDER BY
                                g.TipoMovto,
                                g.Descricao;
                            ";

            var grupos = await ctx.GetListAsync<MapaModel>(sql, reader => new MapaModel
            {
                IDGrupo = reader.GetInt32("IDGrupo"),
                DescGrupo = reader.GetString("DescGrupo"),
                TipoMovto = (TipoMovtoEnum)reader.GetInt32("TipoMovto"),
                TotalMes = new[]
                {
                    reader.GetDecimal("Mes01"),
                    reader.GetDecimal("Mes02"),
                    reader.GetDecimal("Mes03"),
                    reader.GetDecimal("Mes04"),
                    reader.GetDecimal("Mes05"),
                    reader.GetDecimal("Mes06"),
                    reader.GetDecimal("Mes07"),
                    reader.GetDecimal("Mes08"),
                    reader.GetDecimal("Mes09"),
                    reader.GetDecimal("Mes10"),
                    reader.GetDecimal("Mes11"),
                    reader.GetDecimal("Mes12"),
                    0m, // posição 12 reservada
                    0m  // posição 13 reservada
                }
            });
            return grupos;
        }
    }

}

