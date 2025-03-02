using System.Net.Http.Json;
using System.Security;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MediaHandler
{
    class MovieAPISearch
    {
        List<Dictionary<string, string>> FileCollection { get; set; } = new List<Dictionary<string, string>>();
        Dictionary<string, string> SingleFile { get; set; } = new Dictionary<string, string>();

        public async Task<Dictionary<string, string>?> SingleSearch(string file)
        {
            string fileExt = Path.GetExtension(file);
            string searchFile = Path.GetFileNameWithoutExtension(file);
            OmdbapiModel? response = await FetchMedia(file: searchFile).ConfigureAwait(false);

            if (response?.Title != null)
            {
                SingleFile.Add("origin", file);
                string suggestion = $"{response.Title} ({response.Year}){fileExt}";
                SingleFile.Add("result", suggestion);
                return SingleFile;
            }
            else return null;
        }

        private static async Task<OmdbapiModel?> FetchMedia(string file, bool? cleanFile = false)
        {
            string apiKey = "36ff717a";
            string uri = $"/?t={file}&apiKey={apiKey}&plot=short";
            Console.WriteLine($"Searching for {file}");
            try
            {
                Console.WriteLine("Starting Search...");
                var res = await omdbClient.GetFromJsonAsync<OmdbapiModel>(uri);
                Console.WriteLine($"Result Found: {JsonConvert.SerializeObject(res)}");
                return res;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error on Request: {e}");
                file = file.Remove(file.Length - 1, 1);
                if (file.Length == 0)
                {
                    string newFile = Regex.Replace(Regex.Replace(file, @"_\d{2}", "", RegexOptions.IgnoreCase), @"\s*(_|-)\s*", " ", RegexOptions.IgnoreCase);
                    return await FetchMedia(newFile, cleanFile: true).ConfigureAwait(false);
                    // return new OmdbapiModel();
                }
                else if (cleanFile ?? false) return new OmdbapiModel();
                else return await FetchMedia(file).ConfigureAwait(false);
            }
        }

        private static HttpClient omdbClient = new()
        {
            BaseAddress = new Uri("http://www.omdbapi.com")
        };
    }

    public class OmdbapiModel
    {
        public string? Year = null;
        public string? Title = null;
        public string? Rated = null;
        public string? Released = null;
        public string? Runtime = null;
        public string? Genre = null;
        public string? Director = null;
        public string? Writer = null;
        public string? Actors = null;
        public string? Plot = null;
        public string? Poster = null;
        public string? Type = null;
        public string? imdbID = null;
        public string Response = "False";
    }
}