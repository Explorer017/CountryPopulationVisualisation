using System.Text;
using CountryPopulationVisualisation.Models;
using Newtonsoft.Json;

namespace CountryPopulationVisualisation.Services;

public class CountryService
{
    private readonly HttpClient _httpClient;
    
    public CountryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    // get all countries
    public async Task<List<CountryModel>> GetCountries()
    {
        var response = await _httpClient.GetAsync("https://countriesnow.space/api/v0.1/countries/capital");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        AllCountriesModel? countries = JsonConvert.DeserializeObject<AllCountriesModel>(content);
        
        if (countries == null)
        {
            throw new Exception("Failed to deserialize countries");
        }
        
        var flags = await GetFlagList();
        
        // run though each country and add the flag location, as well as set the capital to a placeholder if it is null
        foreach (var country in countries.data)
        {
            var flag = flags.data.FirstOrDefault(x => x.iso2 == country.iso2);
            if (flag != null)
            {
                country.flagLocation = flag.flag;
            }
            
            if (string.IsNullOrEmpty(country.capital))
            {
                country.capital = "(Dataset did not provide capital)";
            }
        }

        countries.data = await UpdateYearsForCountries(countries.data, 2018);
        
        return countries.data;
    }

    public async Task<List<CountryModel>> UpdateYearsForCountries(List<CountryModel> countries, int year)
    {
        AllCountryPopulationModel populationData = await GetAllPopulation();
        foreach (var country in countries)
        {
            // get the population for the country
            var population = populationData.data.FirstOrDefault(x => x.iso3 == country.iso3);
            if (population == null)
            {
                country.population = 0;
            }
            else
            {
                // if the population for the year is not found, set it to 0
                if ((population.populationCounts.FirstOrDefault(x => x.year == year) != null))
                    country.population = population.populationCounts.FirstOrDefault(x => x.year == year).value;
                else
                    country.population = 0;
            }
        }

        return countries;
    }

    private async Task<GetFlagModel> GetFlagList()
    {
        var response = await _httpClient.GetAsync("https://countriesnow.space/api/v0.1/countries/flag/images");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        GetFlagModel? flags = JsonConvert.DeserializeObject<GetFlagModel>(content);
        
        if (flags == null)
        {
            throw new Exception("Failed to deserialize flags");
        }
        
        return flags;
    }

    private async Task<AllCountryPopulationModel> GetAllPopulation()
    {
        var response = await _httpClient.GetAsync("https://countriesnow.space/api/v0.1/countries/population");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        AllCountryPopulationModel? countries = JsonConvert.DeserializeObject<AllCountryPopulationModel>(content);

        if (countries == null)
        {
            throw new Exception("Failed to deserialize countries");
        }
        
        return countries;
    }
    
    public async Task<CountryModel> GetCountry(string iso3)
    {
        var countries = await GetCountries();
        return countries.FirstOrDefault(x => x.iso3 == iso3);
    }


    
}