using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PollerConsoleApp.Data
{
    public interface IRepository
    {
        Task AddNewBatch(IPolledBatch batch);
        Task<IPolledBatch> GetLatestBatch();
    }
}
