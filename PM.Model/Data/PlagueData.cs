using Newtonsoft.Json;

namespace PM.Model.Data
{

public class PlagueData
{
    public string StateId { get; set; }

    public int Level { get; set; }

    [JsonConstructor]
    public PlagueData()
    {
    }

    public PlagueData(string stateId, int level)
    {
        StateId = stateId;
        Level = level;
    }
}

}