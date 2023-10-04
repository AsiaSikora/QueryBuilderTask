using System.Text.RegularExpressions;

namespace QueryBuilderTask.Tests
{
    public static class TestHelper
    {
        public static string JsonStringSource = @"
[
    {
      ""adult"": false,
      ""backdrop_path"": ""/jllyc0pY7xFYLyjuCEP6pO7hm8t.jpg"",
      ""genre_ids"": [
        80,
        35,
        53
      ],
      ""id"": 9430,
      ""original_language"": ""en"",
      ""original_title"": ""Gołł"",
      ""overview"": ""Grocery store clerk Simon occasionally sells drugs from his cash register at work, so when soap opera actors Adam and Zack come looking for Ecstasy on a quiet Christmas Eve, they are surprised to find Ronna covering his shift. Desperate for money, Ronna decides to become an impromptu drug dealer, unaware that Adam and Zack are secretly working for obsessed narcotics officer Burke."",
      ""popularity"": 13.444,
      ""poster_path"": ""/kP0OOAa4GTZSUPW8fgPbk1OmKEW.jpg"",
      ""release_date"": ""1999-04-09"",
      ""title"": ""Go"",
      ""video"": false,
      ""vote_average"": 6.901,
      ""vote_count"": 625
    },
    {
      ""adult"": false,
      ""backdrop_path"": ""/A8paSZwFPs5EmMGE2hbNGKqBfC5.jpg"",
      ""genre_ids"": [
        28,
        14,
        10752
      ],
      ""id"": 856289,
      ""original_language"": ""zh"",
      ""original_title"": ""封神第一部：朝歌风云"",
      ""overview"": ""Based on the most well-known classical fantasy novel of China, Fengshenyanyi, the trilogy is a magnificent eastern high fantasy epic that recreates the prolonged mythical wars between humans, immortals and monsters, which happened more than three thousand years ago."",
      ""popularity"": 159.45,
      ""poster_path"": ""/ccJpK0rqzhQeP7Mrs2uKqObFY4L.jpg"",
      ""release_date"": ""2023-07-20"",
      ""title"": ""Creation of the Gods I: Kingdom of Storms"",
      ""video"": false,
      ""vote_average"": 7.384,
      ""vote_count"": 95
    },
    {
      ""adult"": false,
      ""backdrop_path"": ""/787qS8sM6IpEAJCfggKid0E7ZZo.jpg"",
      ""genre_ids"": [
        10749,
        28,
        18
      ],
      ""id"": 50146,
      ""original_language"": ""zh"",
      ""original_title"": ""金瓶雙艷"",
      ""overview"": ""Golden Lotus is based, in part, on Jin Ping Mei, a famous erotic novel of ancient China. Li Han-Hsiang adapted part of the story into this film, which starts with Hsi Men Ching, a successful merchant, wooing Pan Chin Lien, the beautiful wife of one of the townspeople."",
      ""popularity"": 19.517,
      ""poster_path"": ""/tPGjzPwI0Kh6ymV6uVttw7um7Ft.jpg"",
      ""release_date"": ""1974-01-17"",
      ""title"": ""The Golden Lotus"",
      ""video"": false,
      ""vote_average"": 7.602,
      ""vote_count"": 44
    },
    {
      ""adult"": false,
      ""backdrop_path"": null,
      ""genre_ids"": [],
      ""id"": 313865,
      ""original_language"": ""ja"",
      ""original_title"": ""Go!"",
      ""overview"": ""17 year old Tokyo pizza delivery boy Kosuke crashes his bike into photographer Reiko and breaks her expensive camera. He promises to deliver homemade pizza to make up for her broken camera. After discovering that Reiko has moved 1,200 km away to her hometown of Nagasaki things become trickier, but Kosuke decides to fulfill his promise anyways by borrowing a moped from his workplace."",
      ""popularity"": 2.083,
      ""poster_path"": ""/7AjoubMYq5usswBUzdjKmClXJE2.jpg"",
      ""release_date"": ""2001-10-06"",
      ""title"": ""Go Heat Man!"",
      ""video"": false,
      ""vote_average"": 8.55,
      ""vote_count"": 10
    }
  ]
";
        public static string JsonStringTarget = @"
[
    {
      ""adult"": false,
      ""backdrop_path"": ""/jllyc0pY7xFYLyjuCEP6pO7hm8t.jpg"",
      ""genre_ids"": [
        80,
        35,
        53
      ],
      ""id"": 9430,
      ""original_language"": ""en"",
      ""original_title"": ""Go"",
      ""overview"": ""Grocery store clerk Simon occasionally sells drugs from his cash register at work, so when soap opera actors Adam and Zack come looking for Ecstasy on a quiet Christmas Eve, they are surprised to find Ronna covering his shift. Desperate for money, Ronna decides to become an impromptu drug dealer, unaware that Adam and Zack are secretly working for obsessed narcotics officer Burke."",
      ""popularity"": 13.444,
      ""poster_path"": ""/kP0OOAa4GTZSUPW8fgPbk1OmKEW.jpg"",
      ""release_date"": ""1999-04-09"",
      ""title"": ""Go"",
      ""video"": false,
      ""vote_average"": 6.901,
      ""vote_count"": 625
    },
    {
      ""adult"": false,
      ""backdrop_path"": ""/A8paSZwFPs5EmMGE2hbNGKqBfC5.jpg"",
      ""genre_ids"": [
        28,
        14,
        10752
      ],
      ""id"": 856289,
      ""original_language"": ""zh"",
      ""original_title"": ""封神第一部：朝歌风云"",
      ""overview"": ""Based on the most well-known classical fantasy novel of China, Fengshenyanyi, the trilogy is a magnificent eastern high fantasy epic that recreates the prolonged mythical wars between humans, immortals and monsters, which happened more than three thousand years ago."",
      ""popularity"": 159.45,
      ""poster_path"": ""/ccJpK0rqzhQeP7Mrs2uKqObFY4L.jpg"",
      ""release_date"": ""2023-07-20"",
      ""title"": ""Creation of the Gods I: Kingdom of Storms"",
      ""video"": false,
      ""vote_average"": 7.384,
      ""vote_count"": 95
    },
    {
      ""adult"": false,
      ""backdrop_path"": ""/787qS8sM6IpEAJCfggKid0E7ZZo.jpg"",
      ""genre_ids"": [
        10749,
        28,
        18
      ],
      ""id"": 50146,
      ""original_language"": ""zh"",
      ""original_title"": ""金瓶雙艷"",
      ""overview"": ""Golden Lotus is based, in part, on Jin Ping Mei, a famous erotic novel of ancient China. Li Han-Hsiang adapted part of the story into this film, which starts with Hsi Men Ching, a successful merchant, wooing Pan Chin Lien, the beautiful wife of one of the townspeople."",
      ""popularity"": 19.517,
      ""poster_path"": ""/tPGjzPwI0Kh6ymV6uVttw7um7Ft.jpg"",
      ""release_date"": ""1974-01-17"",
      ""title"": ""The Golden Lotus"",
      ""video"": false,
      ""vote_average"": 7.602,
      ""vote_count"": 44
    },
    {
      ""adult"": false,
      ""backdrop_path"": null,
      ""genre_ids"": [],
      ""id"": 313865,
      ""original_language"": ""ja"",
      ""original_title"": ""Go!"",
      ""overview"": ""17 year old Tokyo pizza delivery boy Kosuke crashes his bike into photographer Reiko and breaks her expensive camera. He promises to deliver homemade pizza to make up for her broken camera. After discovering that Reiko has moved 1,200 km away to her hometown of Nagasaki things become trickier, but Kosuke decides to fulfill his promise anyways by borrowing a moped from his workplace."",
      ""popularity"": 2.083,
      ""poster_path"": ""/7AjoubMYq5usswBUzdjKmClXJE2.jpg"",
      ""release_date"": ""2001-10-06"",
      ""title"": ""Go Heat Man!"",
      ""video"": false,
      ""vote_average"": 8.55,
      ""vote_count"": 10
    }
  ]
";

        public static string RemoveWhitespace(string str)
        {
            return Regex.Replace(str, @"\s", string.Empty);
        }
    }
}
