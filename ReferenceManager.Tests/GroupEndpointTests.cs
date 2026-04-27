using System.Net;
using System.Net.Http.Json;
using ReferenceManager.Models;
using ReferenceManager.Requests;
using ReferenceManager.Responses;
using Xunit;

namespace ReferenceManager.Tests;

public class GroupEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ListGroups_ReturnsOkWithPagedResult()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Group>>("/api/v1/groups");

        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Equal(25, result.Limit);
        Assert.Equal(0, result.Offset);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task ListGroups_IncludesPapersInEachGroup()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Group>>("/api/v1/groups");

        Assert.NotNull(result);
        var seededGroup = result.Items.First();
        Assert.NotNull(seededGroup.Papers);
    }

    [Fact]
    public async Task GetGroup_ExistingId_ReturnsOkWithPapers()
    {
        var response = await _client.GetAsync("/api/v1/groups/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<Group>();
        Assert.NotNull(group);
        Assert.Equal(1, group.Id);
        Assert.NotEmpty(group.Papers);
    }

    [Fact]
    public async Task GetGroup_NonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/groups/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_WithDescription_ReturnsCreatedWithLocation()
    {
        var req = new GroupRequest("Test Group", "A test group");

        var response = await _client.PostAsJsonAsync("/api/v1/groups", req);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var group = await response.Content.ReadFromJsonAsync<Group>();
        Assert.NotNull(group);
        Assert.Equal("Test Group", group.Name);
        Assert.Equal("A test group", group.Description);
    }

    [Fact]
    public async Task CreateGroup_WithoutDescription_ReturnsCreated()
    {
        var req = new GroupRequest("Tag-style Group", null);

        var response = await _client.PostAsJsonAsync("/api/v1/groups", req);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<Group>();
        Assert.NotNull(group);
        Assert.Equal("Tag-style Group", group.Name);
        Assert.Null(group.Description);
    }

    [Fact]
    public async Task UpdateGroup_ExistingId_ReturnsOkWithUpdatedData()
    {
        var req = new GroupRequest("Updated Name", "Updated description");

        var response = await _client.PutAsJsonAsync("/api/v1/groups/1", req);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<Group>();
        Assert.NotNull(group);
        Assert.Equal("Updated Name", group.Name);
    }

    [Fact]
    public async Task UpdateGroup_NonExistentId_ReturnsNotFound()
    {
        var req = new GroupRequest("Name", null);

        var response = await _client.PutAsJsonAsync("/api/v1/groups/99999", req);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_NonExistentId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/groups/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPaperToGroup_AlreadyInGroup_ReturnsConflict()
    {
        // Paper 1 (HAnS SPLC 2021) is already in group 1 (Accepted Papers) via seeder
        var response = await _client.PostAsync("/api/v1/groups/1/papers/1", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddPaperToGroup_NonExistentGroup_ReturnsNotFound()
    {
        var response = await _client.PostAsync("/api/v1/groups/99999/papers/1", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPaperToGroup_NonExistentPaper_ReturnsNotFound()
    {
        var response = await _client.PostAsync("/api/v1/groups/1/papers/99999", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemovePaperFromGroup_NonExistentPaper_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/groups/1/papers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddThenRemovePaperFromGroup_ReturnsNoContent()
    {
        // Paper 2 is not in group 1 — add it, then remove it
        var add = await _client.PostAsync("/api/v1/groups/1/papers/2", null);
        Assert.Equal(HttpStatusCode.NoContent, add.StatusCode);

        var remove = await _client.DeleteAsync("/api/v1/groups/1/papers/2");
        Assert.Equal(HttpStatusCode.NoContent, remove.StatusCode);
    }
}
