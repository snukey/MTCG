using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Models;

namespace HttpServer.DAL
{
    public interface ITradeRepository
    {
        void InsertTrade();
        List<Trade> GetTrades();
    }
}
