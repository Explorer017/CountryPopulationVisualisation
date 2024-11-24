using System.Text;
using CountryPopulationVisualisation.Models;
using Newtonsoft.Json;

namespace CountryPopulationVisualisation.Services;

public class CityService
{
    private readonly HttpClient _httpClient;
    
    public CityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Method <c>getCityPopulation</c> calls the API to get all city population data for a given country
    /// </summary>
    private async Task<GetCityPopulationModel> getCityPopulation(string country)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://countriesnow.space/api/v0.1/countries/population/cities/filter");
        request.Content = new StringContent($"{{ \"country\":\"{country}\", \"order\":\"asc\", \"orderBy\":\"name\" }}", Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        GetCityPopulationModel cityPopulation = JsonConvert.DeserializeObject<GetCityPopulationModel>(content);
        
        if (cityPopulation == null)
        {
            throw new ApplicationException("Failed to deserialize city population");
        }
        
        return cityPopulation;
    }

    /// <summary>
    /// Method <c>GetCities</c> returns a list of <c>City</c> objects, representing all cities in a given country,
    /// containing all population data for that city
    /// </summary>
    public async Task<List<City>?> GetCities(string country)
    {
        GetCityPopulationModel cityPopulation = await getCityPopulation(country);
        if (cityPopulation == null || cityPopulation.data == null)
        {
            return null;
        }
        List<City> cities = new List<City>();

        foreach (CityPopulationModel cityPopulationModel in cityPopulation.data)
        {
            City city = new City(cityPopulationModel.city, cityPopulationModel.country);
            foreach (var populations in cityPopulationModel.populationCounts)
            {
                // if year already exists, replace with new data only if population is higher
                if (city.Population.ContainsKey(populations.year))
                {
                    if (city.Population[populations.year] < populations.value)
                    {
                        city.Population[populations.year] = populations.value;
                    }
                }
                else
                {
                    city.Population.Add(populations.year, populations.value);
                }
            }
            // check the country is correct
            // needs to be equals to or starts with because of an edge case with "United Kingdom" where the country
            // name is "United Kingdom" but the country name on the city is "United Kingdom of Great Britain and Northern Ireland"
            if (city.Country == country || city.Country.StartsWith($@"{country} "))
            {
                cities.Add(city);
            }
        }

        if (cities.Count == 0)
        {
            return null;
        }

        return cities;
    }
}