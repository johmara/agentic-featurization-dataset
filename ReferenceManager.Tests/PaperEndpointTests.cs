using System.Net;
using System.Net.Http.Json;
using ReferenceManager.Models;
using ReferenceManager.Requests;
using ReferenceManager.Responses;
using Xunit;

namespace ReferenceManager.Tests;

public class PaperEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ListPapers_ReturnsOkWithPagedResult()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Paper>>("/api/v1/papers");

        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Equal(25, result.Limit);
        Assert.Equal(0, result.Offset);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task ListPapers_LimitQueryParam_CappsReturnedItems()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Paper>>("/api/v1/papers?limit=3");

        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(3, result.Limit);
    }

    [Fact]
    public async Task ListPapers_OffsetQueryParam_SkipsItems()
    {
        var all = await _client.GetFromJsonAsync<PagedResult<Paper>>("/api/v1/papers?limit=100");
        var paged = await _client.GetFromJsonAsync<PagedResult<Paper>>("/api/v1/papers?limit=100&offset=1");

        Assert.NotNull(all);
        Assert.NotNull(paged);
        Assert.Equal(all.Total, paged.Total);
        Assert.Equal(all.Items.Skip(1).First().Id, paged.Items.First().Id);
    }

    [Fact]
    public async Task GetPaper_ExistingId_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/papers/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paper = await response.Content.ReadFromJsonAsync<Paper>();
        Assert.NotNull(paper);
        Assert.Equal(1, paper.Id);
    }

    [Fact]
    public async Task GetPaper_NonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/papers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePaper_ValidRequest_ReturnsCreatedWithLocation()
    {
        var req = new PaperRequest(
            "Test Paper",
            [new AuthorRequest("Test Author", null, [])],
            2024, null, null, null, null);

        var response = await _client.PostAsJsonAsync("/api/v1/papers", req);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var paper = await response.Content.ReadFromJsonAsync<Paper>();
        Assert.NotNull(paper);
        Assert.Equal("Test Paper", paper.Title);
    }

    [Fact]
    public async Task UpdatePaper_ExistingId_ReturnsOkWithUpdatedData()
    {
        var req = new PaperRequest(
            "Updated Title",
            [new AuthorRequest("Author", null, [])],
            2025, "Updated abstract", null, null, null);

        var response = await _client.PutAsJsonAsync("/api/v1/papers/1", req);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paper = await response.Content.ReadFromJsonAsync<Paper>();
        Assert.NotNull(paper);
        Assert.Equal("Updated Title", paper.Title);
    }

    [Fact]
    public async Task UpdatePaper_NonExistentId_ReturnsNotFound()
    {
        var req = new PaperRequest("Title", [], 2024, null, null, null, null);

        var response = await _client.PutAsJsonAsync("/api/v1/papers/99999", req);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePaper_NonExistentId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/papers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
