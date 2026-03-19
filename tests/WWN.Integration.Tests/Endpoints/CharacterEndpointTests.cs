using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WWN.Application.DTOs;
using WWN.Infrastructure.Persistence;

namespace WWN.Integration.Tests.Endpoints;

public class CharacterEndpointTests : IClassFixture<CharacterEndpointTests.CustomFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly SqliteConnection _connection;

    public class CustomFactory : WebApplicationFactory<Program>
    {
        public SqliteConnection Connection { get; }

        public CustomFactory()
        {
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<WwnDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<WwnDbContext>(opt =>
                    opt.UseSqlite(Connection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Connection.Dispose();
            base.Dispose(disposing);
        }
    }

    public CharacterEndpointTests(CustomFactory factory)
    {
        _connection = factory.Connection;
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private async Task<Guid> CreateTestCharacter()
    {
        var req = new CreateCharacterRequest
        {
            Name = "Test Hero",
            Class = "Warrior",
            Attributes = new Dictionary<string, int>
            {
                ["Strength"] = 14,
                ["Dexterity"] = 12,
                ["Constitution"] = 10,
                ["Intelligence"] = 10,
                ["Wisdom"] = 10,
                ["Charisma"] = 10
            }
        };
        var response = await _client.PostAsJsonAsync("/api/characters", req);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<IdResponse>();
        return result!.Id;
    }

    [Fact]
    public async Task POST_CreateCharacter_Returns201()
    {
        var id = await CreateTestCharacter();
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GET_ListCharacters_ReturnsArray()
    {
        await CreateTestCharacter();
        var response = await _client.GetAsync("/api/characters");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<CharacterSummaryDto>>();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GET_GetCharacter_ReturnsDetailWithDerivedStats()
    {
        var id = await CreateTestCharacter();
        var response = await _client.GetAsync($"/api/characters/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Test Hero");
        dto.DerivedStats.Should().NotBeNull();
        dto.DerivedStats.ArmorClass.Should().BeGreaterThan(0);
        dto.Attributes.Should().HaveCount(6);
        dto.Skills.Should().HaveCount(16);
    }

    [Fact]
    public async Task GET_NonExistent_Returns404()
    {
        var response = await _client.GetAsync($"/api/characters/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_UpdateAttribute_RecalculatesDerived()
    {
        var id = await CreateTestCharacter();
        var initial = await _client.GetFromJsonAsync<CharacterDetailDto>($"/api/characters/{id}");
        var initialAc = initial!.DerivedStats.ArmorClass;

        var response = await _client.PutAsJsonAsync($"/api/characters/{id}/attributes/Dexterity",
            new UpdateAttributeRequest { Score = 18 });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        updated!.DerivedStats.ArmorClass.Should().BeGreaterThan(initialAc);
    }

    [Fact]
    public async Task PUT_SetHp_Returns200()
    {
        var id = await CreateTestCharacter();
        var response = await _client.PutAsJsonAsync($"/api/characters/{id}/hp",
            new SetHpRequest { MaxHitPoints = 20, CurrentHitPoints = 15 });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto!.MaxHitPoints.Should().Be(20);
        dto.CurrentHitPoints.Should().Be(15);
    }

    [Fact]
    public async Task DELETE_Character_Returns204()
    {
        var id = await CreateTestCharacter();
        var response = await _client.DeleteAsync($"/api/characters/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task POST_AddWeapon_ShowsInInventory()
    {
        var id = await CreateTestCharacter();
        var addResponse = await _client.PostAsJsonAsync($"/api/characters/{id}/items",
            new AddItemRequest
            {
                Name = "Longsword",
                Encumbrance = 1,
                ItemType = "Weapon",
                DamageDieCount = 1,
                DamageDieSides = 8,
                AttributeModifier = "Strength",
                Tags = "Melee"
            });
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await addResponse.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto!.Inventory.Should().HaveCount(1);
        dto.Inventory[0].ItemType.Should().Be("Weapon");
    }

    private record IdResponse(Guid Id);
}
