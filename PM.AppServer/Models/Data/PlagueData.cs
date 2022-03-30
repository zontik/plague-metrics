using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PM.AppServer.Models.Data
{

public class PlagueData
{
    public string StateId { get; set; }

    public int Level { get; set; }

    [JsonConstructor]
    public PlagueData()
    {
    }

    public PlagueData(JToken jToken, string tokenPath)
    {
        StateId = jToken.Value<string>("state");
        Level = (int)jToken.SelectToken(tokenPath, true);
    }
}

}