using System;
using Newtonsoft.Json.Linq;

namespace PM.AppServer.Models.Data
{

public abstract class PlagueData
{
    public string State { get; set; }

    public double Level { get; set; }

    protected abstract void Init(JToken json);

    public static PlagueData CreatePlagueData(string typeKey, JToken json)
    {
        PlagueData plagueData = typeKey switch
        {
            "overall" => new OverallPlagueData(),
            "cases" => new CasesPlagueData(),
            "test" => new TestPlagueData(),
            "infection" => new InfectionPlagueData(),
            _ => throw new ArgumentOutOfRangeException(nameof(typeKey))
        };

        plagueData.Init(json);
        return plagueData;
    }

    public class OverallPlagueData : PlagueData
    {
        protected override void Init(JToken json)
        {
            InitInternal(json, "overall");
        }
    }

    public class CasesPlagueData : PlagueData
    {
        protected override void Init(JToken json)
        {
            InitInternal(json, "caseDensity");
        }
    }

    public class TestPlagueData : PlagueData
    {
        protected override void Init(JToken json)
        {
            InitInternal(json, "testPositivityRatio");
        }
    }

    public class InfectionPlagueData : PlagueData
    {
        protected override void Init(JToken json)
        {
            InitInternal(json, "infectionRate");
        }
    }

    private void InitInternal(JToken json, string property)
    {
        State = json.Value<string>("state");
        Level = GetLevel(json, property);
    }

    private static double GetLevel(JToken json, string property)
    {
        var riskLevels = json["riskLevels"];
        return riskLevels.Value<double>(property);
    }
}

}