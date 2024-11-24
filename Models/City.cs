namespace CountryPopulationVisualisation.Models;

public class City
{
    public string Name { get; set; }
    public string Country { get; set; }
    public Dictionary<int, double> Population { get; set; }

    public City(string name, string country)
    {
        Name = name;
        Country = country;
        Population = new Dictionary<int, double>();
    }

    /// <summary>
    /// Method <c>GetYears</c> returns all years that have data for a list of Cities
    /// </summary>
    public static List<int> GetYears(List<City>? cities)
    {
        if (cities == null)
            return new List<int>();
        List<int> years = new List<int>();
        foreach (var city in cities)
        {
            foreach (KeyValuePair<int, double> population in city.Population)
            {
                if (!years.Contains(population.Key))
                {
                    years.Add(population.Key);
                }
            }
        }

        years.Sort();
        return years;
    }
}