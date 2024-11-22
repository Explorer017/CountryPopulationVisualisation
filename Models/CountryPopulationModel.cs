namespace CountryPopulationVisualisation.Models;

public class CountryPopulationModel
{
    public CountryPopulation data { get; set; }
}

public class CountryPopulation
{
    public string iso3 { get; set; }
    public List<PopulationData> populationCounts { get; set; }

    public class PopulationData
    {
        public int year { get; set; }
        public long value { get; set; }
    }
}