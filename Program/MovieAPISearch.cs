using System.Net.Http.Json;
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
            OmdbapiModel response = await FetchMedia(file: searchFile).ConfigureAwait(false);

            if (response.Title != null)
            {
                SingleFile.Add("origin", file);
                string suggestion = $"{response.Title} ({response.Year}){fileExt}";
                SingleFile.Add("result", suggestion);
                return SingleFile;
            }
            else return null;
        }

        private static async Task<OmdbapiModel> FetchMedia(string file, bool? cleanFile = false)
        {
            string apiKey = "36ff717a";
            string uri = $"http://www.omdbapi.com/?t={file}&apiKey={apiKey}&plot=short";
            Console.WriteLine($"Searching for {file}");
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Console.WriteLine("Starting Search...");
                    string res = await client.GetStringAsync(uri);
                    OmdbapiModel resBody = JsonConvert.DeserializeObject<OmdbapiModel>(res) ?? new OmdbapiModel();
                    Console.WriteLine($"Result Found: {JsonConvert.SerializeObject(resBody)}");
                    return resBody;
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
        }
    }

    public class OmdbapiModel
    {
        public string? Year { get; set; }
        public string? Title { get; set; }
        public string? Rated { get; set; }
        public string? Released { get; set; }
        public string? Runtime { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public string? Writer { get; set; }
        public string? Actors { get; set; }
        public string? Plot { get; set; }
        public string? Poster { get; set; }
        public string? Type { get; set; }
        public string? imdbID { get; set; }
        public string Response { get; set; } = "False";
    }
}