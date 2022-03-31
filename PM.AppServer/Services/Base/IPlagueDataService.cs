using System.Collections.Generic;
using System.Threading.Tasks;
using PM.Model.Data;

namespace PM.AppServer.Services.Base
{

public interface IPlagueDataService
{
    Task<IEnumerable<PlagueData>> ListData(string tokenPath);
}

}