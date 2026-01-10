using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGMPulse.Domain.ViewModels;

public class BaixarAgendaModel
{
    public int IDAgenda { get; set; }
    public DateTime DataPagameto { get; set; }
    public DateTime DataVencto { get; set; }
}
