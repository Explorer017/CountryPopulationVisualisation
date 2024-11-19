namespace CountryPopulationVisualisation.Models;

public class AllCountriesModel
{
    public string error { get; set; }
    public string msg { get; set; }
    public List<CountryModel> data { get; set; }
}