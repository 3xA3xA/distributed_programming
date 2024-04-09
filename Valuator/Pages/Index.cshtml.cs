using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System.Diagnostics;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDatabase _database;
    private readonly IServer _server;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _database = connectionMultiplexer.GetDatabase();
        _server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
    }

    public void OnGet()
    {

    }

    public IActionResult? OnPost(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return BadRequest();//null;
        }

        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        _database.StringSet(rankKey, rank.ToString());

        string similarityKey = "SIMILARITY-" + id;
        double similarity = CalculateSimilarity(text);
        _database.StringSet(similarityKey, similarity.ToString());

        string textKey = "TEXT-" + id;
        _database.StringSet(textKey, text);

        return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)
    {
        int nonAlphabeticCharacters = text.Count(c => !char.IsLetter(c));
        double rank = (double)nonAlphabeticCharacters / text.Length;
        return rank;
    }

    private double CalculateSimilarity(string text)
    {
        var keys = _server.Keys(pattern: "TEXT-*");
        foreach (var key in keys)
        {
            string? storedText = _database.StringGet(key);
            if (storedText == text)
            {
                Console.WriteLine($"{storedText} = {text}");
                return 1.0;
            }
            Console.WriteLine(_database.StringGet(key));
        }
        return 0.0;
    }
}
