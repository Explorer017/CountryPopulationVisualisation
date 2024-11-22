namespace CountryPopulationVisualisation.Models;

public class GetFlagModel
{
    
    public string error { get; set; }
    public string msg { get; set; }

    public FlagData data { get; set; }
    
}
public class GetFlagsModel
{
    public string error { get; set; }
    public string msg { get; set; }
    
    public List<FlagData> data { get; set; }
    
}

public class FlagData
{
    public string flag { get; set; }
    public string iso2 { get; set; }
}