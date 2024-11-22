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

    private async Task<GetCityPopulationModel> getCityPopulation(string country)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://countriesnow.space/api/v0.1/countries/population/cities/filter");
        request.Content = new StringContent($"{{ \"country\":\"{country}\", \"order\":\"asc\", \"orderBy\":\"name\" }}", Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        GetCityPopulationModel cityPopulation = JsonConvert.DeserializeObject<GetCityPopulationModel>(content);
        
        if (cityPopulation == null)
        {
            throw new ApplicationException("Failed to deserialize city population");
        }
        
        return cityPopulation;
    }

    public async Task<List<City>> GetCities(string country)
    {
        GetCityPopulationModel cityPopulation = await getCityPopulation(country);
        List<City> cities = new List<City>();

        foreach (CityPopulationModel cityPopulationModel in cityPopulation.data)
        {
            City city = new City(cityPopulationModel.city);
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
            cities.Add(city);
        }

        if (cities.Count == 0)
        {
            return null;
        }

        return cities;
    }
}