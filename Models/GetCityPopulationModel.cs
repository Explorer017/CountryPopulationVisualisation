namespace CountryPopulationVisualisation.Models;

public class GetCityPopulationModel
{
    public List<CityPopulationModel> data { get; set; }
}

public class CityPopulationModel
{
    public string city { get; set; }
    public string country { get; set; }
    public List<CityPopulationCountsModel> populationCounts { get; set; }
    public class CityPopulationCountsModel
    {
        public int year { get; set; }
        public double value { get; set; }
    }
}