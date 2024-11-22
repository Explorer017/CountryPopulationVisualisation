namespace CountryPopulationVisualisation.Models;

public class City
{
    public string Name { get; set; }
    public Dictionary<int, long> Population { get; set; }

    public City(string name)
    {
        Name = name;
        Population = new Dictionary<int, long>();
    }
}