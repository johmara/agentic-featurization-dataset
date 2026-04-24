using System.Net;
using System.Net.Http.Json;
using ReferenceManager.Models;
using ReferenceManager.Requests;
using ReferenceManager.Responses;
using Xunit;

namespace ReferenceManager.Tests;

public class CollectionEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ListCollections_ReturnsOkWithPagedResult()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Collection>>("/api/v1/collections");

        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Equal(25, result.Limit);
        Assert.Equal(0, result.Offset);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task ListCollections_IncludesPapersInEachCollection()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Collection>>("/api/v1/collections");

        Assert.NotNull(result);
        var seededCollection = result.Items.First();
        Assert.NotNull(seededCollection.Papers);
    }

    [Fact]
    public async Task GetCollection_ExistingId_ReturnsOkWithPapers()
    {
        var response = await _client.GetAsync("/api/v1/collections/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var col = await response.Content.ReadFromJsonAsync<Collection>();
        Assert.NotNull(col);
        Assert.Equal(1, col.Id);
        Assert.NotEmpty(col.Papers);
    }

    [Fact]
    public async Task GetCollection_NonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/collections/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCollection_ValidRequest_ReturnsCreatedWithLocation()
    {
        var req = new CollectionRequest("Test Collection", "A test collection");

        var response = await _client.PostAsJsonAsync("/api/v1/collections", req);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var col = await response.Content.ReadFromJsonAsync<Collection>();
        Assert.NotNull(col);
        Assert.Equal("Test Collection", col.Name);
        Assert.Equal("A test collection", col.Description);
    }

    [Fact]
    public async Task UpdateCollection_ExistingId_ReturnsOkWithUpdatedData()
    {
        var req = new CollectionRequest("Updated Name", "Updated description");

        var response = await _client.PutAsJsonAsync("/api/v1/collections/1", req);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var col = await response.Content.ReadFromJsonAsync<Collection>();
        Assert.NotNull(col);
        Assert.Equal("Updated Name", col.Name);
    }

    [Fact]
    public async Task UpdateCollection_NonExistentId_ReturnsNotFound()
    {
        var req = new CollectionRequest("Name", null);

        var response = await _client.PutAsJsonAsync("/api/v1/collections/99999", req);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCollection_NonExistentId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/collections/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPaperToCollection_AlreadyInCollection_ReturnsConflict()
    {
        // Paper 1 (HAnS SPLC 2021) is already in collection 1 (Accepted Papers) via seeder
        var response = await _client.PostAsync("/api/v1/collections/1/papers/1", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddPaperToCollection_NonExistentCollection_ReturnsNotFound()
    {
        var response = await _client.PostAsync("/api/v1/collections/99999/papers/1", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPaperToCollection_NonExistentPaper_ReturnsNotFound()
    {
        var response = await _client.PostAsync("/api/v1/collections/1/papers/99999", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemovePaperFromCollection_NonExistentPaper_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/collections/1/papers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddThenRemovePaperFromCollection_ReturnsNoContent()
    {
        // Paper 2 is not in collection 1 — add it, then remove it
        var add = await _client.PostAsync("/api/v1/collections/1/papers/2", null);
        Assert.Equal(HttpStatusCode.NoContent, add.StatusCode);

        var remove = await _client.DeleteAsync("/api/v1/collections/1/papers/2");
        Assert.Equal(HttpStatusCode.NoContent, remove.StatusCode);
    }
}
