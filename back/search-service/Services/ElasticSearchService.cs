using Nest;
using SearchService.Models;
using SearchService.DTOs;

namespace SearchService.Services;

public class ElasticSearchService
{
    private readonly IElasticClient _client;

    public ElasticSearchService(IElasticClient client)
    {
        _client = client;
    }

    public async Task IndexAudioAsync(AudioRecord record)
    {
        await _client.IndexDocumentAsync(record);
    }

    public async Task<List<SearchResult>> SearchAsync(string query, int fuzziness = 1)
    {
        var searchResponse = await _client.SearchAsync<AudioRecord>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.SongName)
                    .Query(query)
                    .Analyzer("russian")
                    .Fuzziness(Fuzziness.EditDistance(fuzziness))
                )
            )
            .Highlight(h => h
                .Fields(f => f
                    .Field(ff => ff.SongName)
                    .PreTags("<em>")
                    .PostTags("</em>")
                )
            )
        );

        return searchResponse.Hits.Select(h => new SearchResult
        {
            Id = h.Source.Id,
            SongName = h.Source.SongName,
            Author = h.Source.Author,
            UploadDate = h.Source.UploadDate,
            Score = h.Score,
            Highlights = h.Highlight?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToList()
            )
        }).ToList();
    }
}