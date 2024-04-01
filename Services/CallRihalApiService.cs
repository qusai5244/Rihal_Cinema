using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Models;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Services
{
    public class CallRihalApiService : ICallRihalApiService
    {
        private readonly HttpClient _httpClient;

        public CallRihalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Movie>> GetMoviesByIdsAsync()
        {
            try
            {
                var moviesJson = File.ReadAllText("Data/movies.json");
                var moviesList = JsonConvert.DeserializeObject<List<Movie>>(moviesJson);

                List<Movie> movies = new List<Movie>();
                List<MainCast> mainCasts;
                RihalApiResults deserializedData;
                DateTime? releaseDate;

                for (int i = 0; i < moviesList.Count(); i++) // I adjusted the loop indexes based on your original code
                {
                    int currentId = i;

                    // Construct the URL
                    string apiUrl = $"https://cinema.stag.rihal.tech/api/movie/{currentId + 1}";

                    // Make the API call and retrieve the response
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string data = await response.Content.ReadAsStringAsync();
                        deserializedData = JsonConvert.DeserializeObject<RihalApiResults>(data);

                        mainCasts = new List<MainCast>(); // Initialize mainCasts for each movie

                        foreach (var mainCast in deserializedData.MainCasts)
                        {
                            mainCasts.Add(new MainCast
                            {
                                Name = mainCast,
                                MovieId = currentId + 1,
                            });
                        }

                        if (!string.IsNullOrEmpty(deserializedData.ReleaseDate)){
                            releaseDate = ParseDateString(deserializedData.ReleaseDate);
                        }
                        else
                        {
                            releaseDate = null;
                        }

                        movies.Add(new Movie
                        {
                            Name = moviesList[i].Name,
                            Description = moviesList[i].Description,
                            ReleaseDate = releaseDate,
                            Director = string.IsNullOrEmpty(deserializedData.Director) ? null : deserializedData.Director,
                            Budget = deserializedData.Budget == 0 ? null : deserializedData.Budget, // no need to check for 0, let it be assigned as is
                            MainCasts = mainCasts // Assign the mainCasts list to the movie
                        });
                    }
                    else
                    {
                        // Handle the error response here (e.g., logging, continue with next iteration)
                        Console.WriteLine($"Failed to fetch data for ID {currentId}. Status code: {response.StatusCode}");
                    }
                }

                return movies;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }

        }

        //TODO : Check this
        private static DateTime ParseDateString(string dateString)
        {
            string format = "dd-MM-yyyy";
            DateTime dateTime;
            if (DateTime.TryParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                // Convert the parsed DateTime value to UTC
                return dateTime.ToUniversalTime().AddHours(8);
            }
            else
            {
                // You may handle the failure case differently, such as throwing an exception or returning a default value
                throw new ArgumentException("Invalid date string format", nameof(dateString));
            }
        }

    }
}
