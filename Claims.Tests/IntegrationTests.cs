using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.Extensions;
using Claims.Application.Models;
using Claims.Domain.Entities;
using Claims.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Claims.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        [Fact]
        public async Task CoversAndClaims_IntegrationWorkflow()
        {
            // 1. Create a Cover
            var startDate = DateTimeExtensions.UtcToday().AddDays(1);
            var endDate = startDate.AddDays(30);
            var createCoverModel = new CreateCoverRequestModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Type = CoverType.Yacht
            };

            var createCoverResponse = await _client.PostAsJsonAsync("/Covers", createCoverModel);
            createCoverResponse.EnsureSuccessStatusCode();
            var createdCover = await createCoverResponse.Content.ReadFromJsonAsync<CoverModel>(_jsonOptions);
            Assert.NotNull(createdCover);
            Assert.NotNull(createdCover.Id);

            // 2. Get the created Cover
            var getCoverResponse = await _client.GetAsync($"/Covers/{createdCover.Id}");
            getCoverResponse.EnsureSuccessStatusCode();
            var fetchedCover = await getCoverResponse.Content.ReadFromJsonAsync<CoverModel>(_jsonOptions);
            Assert.Equal(createdCover.Id, fetchedCover?.Id);
            
            // 3. Create a Claim for this Cover
            var createClaimModel = new CreateClaimRequestModel
            {
                CoverId = createdCover.Id,
                Name = "Integration Test Claim",
                Type = ClaimType.Fire,
                DamageCost = 5000,
                Created = startDate.AddDays(5)
            };

            var createClaimResponse = await _client.PostAsJsonAsync("/Claims", createClaimModel);
            createClaimResponse.EnsureSuccessStatusCode();
            var createdClaim = await createClaimResponse.Content.ReadFromJsonAsync<ClaimModel>(_jsonOptions);
            Assert.NotNull(createdClaim);
            Assert.Equal(createdCover.Id, createdClaim.CoverId);

            // 4. Get all Claims
            var getClaimsResponse = await _client.GetAsync("/Claims");
            getClaimsResponse.EnsureSuccessStatusCode();
            var claims = await getClaimsResponse.Content.ReadFromJsonAsync<IEnumerable<ClaimModel>>(_jsonOptions);
            Assert.Contains(claims!, c => c.Id == createdClaim.Id);

            // 5. Get the created Claim
            var getClaimResponse = await _client.GetAsync($"/Claims/{createdClaim.Id}");
            getClaimResponse.EnsureSuccessStatusCode();
            var fetchedClaim = await getClaimResponse.Content.ReadFromJsonAsync<ClaimModel>(_jsonOptions);
            Assert.Equal(createdClaim.Id, fetchedClaim?.Id);

            // 7. Delete Claim
            var deleteClaimResponse = await _client.DeleteAsync($"/Claims/{createdClaim.Id}");
            deleteClaimResponse.EnsureSuccessStatusCode();

            // 8. Delete Cover
            var deleteCoverResponse = await _client.DeleteAsync($"/Covers/{createdCover.Id}");
            deleteCoverResponse.EnsureSuccessStatusCode();
            
            // 9. Verify deletion
            var getDeletedClaimResponse = await _client.GetAsync($"/Claims/{createdClaim.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getDeletedClaimResponse.StatusCode);

            var getDeletedCoverResponse = await _client.GetAsync($"/Covers/{createdCover.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getDeletedCoverResponse.StatusCode);

            // Verify background jobs
            using var scope = _factory.Services.CreateScope();
            var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();
            
            // add delay to be sure to background jobs are finished
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            //covers audits
            var coverAuditList = await auditContext.CoverAudits.ToListAsync();
            Assert.NotEmpty(coverAuditList);
            Assert.Contains(coverAuditList, a => a.HttpRequestType == "POST" && a.CoverId == createdCover.Id);
            Assert.Contains(coverAuditList, a => a.HttpRequestType == "DELETE" && a.CoverId == createdCover.Id);
            
            //claims audits
            var claimAuditList = await auditContext.CoverAudits.ToListAsync();
            Assert.NotEmpty(claimAuditList);
            Assert.Contains(claimAuditList, a => a.HttpRequestType == "POST" && a.CoverId == createdCover.Id);
            Assert.Contains(claimAuditList, a => a.HttpRequestType == "DELETE" && a.CoverId == createdCover.Id);
        }

        [Fact]
        public async Task CreateCover_ShouldReturnBadRequest_WhenStartDateInPast()
        {
            var createCoverModel = new CreateCoverRequestModel
            {
                StartDate = DateTimeExtensions.UtcToday().AddDays(-1),
                EndDate = DateTimeExtensions.UtcToday().AddDays(10),
                Type = CoverType.Yacht
            };

            var response = await _client.PostAsJsonAsync("/Covers", createCoverModel);
            
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateClaim_ShouldReturnBadRequest_WhenOutsideCoverPeriod()
        {
            // 1. Create a Cover
            var startDate = DateTimeExtensions.UtcToday().AddDays(1);
            var endDate = startDate.AddDays(30);
            var createCoverModel = new CreateCoverRequestModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Type = CoverType.Yacht
            };

            var coverResponse = await _client.PostAsJsonAsync("/Covers", createCoverModel);
            var cover = await coverResponse.Content.ReadFromJsonAsync<CoverModel>(_jsonOptions);

            // 2. Create a Claim with date BEFORE cover starts
            var createClaimModel = new CreateClaimRequestModel
            {
                CoverId = cover!.Id,
                Name = "Invalid Date Claim",
                Type = ClaimType.Fire,
                DamageCost = 5000,
                Created = startDate.AddDays(-1)
            };

            var claimResponse = await _client.PostAsJsonAsync("/Claims", createClaimModel);
            
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, claimResponse.StatusCode);
        }
        
        [Fact]
        public async Task Premium_Compute_ShouldReturnCorrectValue()
        {
            var request = new Claims.Application.Models.PremiumComputeRequestModel
            {
                StartDate = DateTimeExtensions.UtcToday(),
                EndDate = DateTimeExtensions.UtcToday().AddDays(25),
                Type = CoverType.Yacht
            };

            var response = await _client.PostAsJsonAsync($"/Premium/compute", request);
            response.EnsureSuccessStatusCode();
            var premium = await response.Content.ReadFromJsonAsync<decimal>(_jsonOptions);

            Assert.Equal(34375.0m, premium);
        }
    }
}
