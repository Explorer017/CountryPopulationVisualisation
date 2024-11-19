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
        
        return countries.data;
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


    
}