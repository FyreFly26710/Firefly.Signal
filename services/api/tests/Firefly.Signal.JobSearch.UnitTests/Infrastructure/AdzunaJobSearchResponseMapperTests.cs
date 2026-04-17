using System.Text.Json;
using Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.UnitTests.Infrastructure;

[TestClass]
public sealed class AdzunaJobSearchResponseMapperTests
{
    [TestMethod]
    public void Map_WhenAdzunaReturnsSalaryAndMetadata_PreservesThemOnJobPosting()
    {
        var mapper = new AdzunaJobSearchResponseMapper();
        var request = new AdzunaJobSearchRequest(
            PageNumber: 1,
            ResultsPerPage: 50,
            What: "Developer",
            WhatExclude: null,
            Where: "london",
            Distance: 5,
            Category: "it-jobs",
            SalaryMin: null,
            SalaryMax: null,
            FullTime: null,
            PartTime: null,
            Permanent: null,
            Contract: null,
            SortBy: null,
            MaxDaysOld: 30,
            Company: null,
            TitleOnly: false,
            Location0: null,
            Location1: null,
            Location2: null);

        var response = new AdzunaJobSearchResponse
        {
            Count = 1,
            Results =
            [
                new AdzunaJobSearchResultResponse
                {
                    Id = "123",
                    AdReference = "ad-456",
                    Title = "Senior Developer",
                    Description = "Remote .NET role",
                    RedirectUrl = "https://example.com/jobs/123",
                    Created = "2026-04-17T08:00:00Z",
                    Latitude = 51.5074m,
                    Longitude = -0.1278m,
                    SalaryMin = 70000m,
                    SalaryMax = 90000m,
                    SalaryIsPredicted = "1",
                    ContractTime = "full_time",
                    ContractType = "permanent",
                    Company = new AdzunaCompanyResponse
                    {
                        DisplayName = "Acme Ltd",
                        CanonicalName = "acme-ltd"
                    },
                    Category = new AdzunaCategoryResponse
                    {
                        Tag = "it-jobs",
                        Label = "IT Jobs"
                    },
                    Location = new AdzunaLocationResponse
                    {
                        DisplayName = "The City, Central London",
                        Area = ["UK", "London", "Central London", "The City"]
                    }
                }
            ]
        };

        var result = mapper.Map(response, request);
        var job = result.Jobs.Single();

        Assert.AreEqual(70000m, job.SalaryMin);
        Assert.AreEqual(90000m, job.SalaryMax);
        Assert.IsTrue(job.SalaryIsPredicted.HasValue);
        Assert.IsTrue(job.SalaryIsPredicted.Value);
        Assert.AreEqual("full_time", job.ContractTime);
        Assert.AreEqual("permanent", job.ContractType);
        Assert.IsTrue(job.IsFullTime);
        Assert.IsFalse(job.IsPartTime);
        Assert.IsTrue(job.IsPermanent);
        Assert.IsFalse(job.IsContract);
        Assert.AreEqual("123", job.SourceJobId);
        Assert.AreEqual("ad-456", job.SourceAdReference);
        Assert.AreEqual("Acme Ltd", job.CompanyDisplayName);
        Assert.AreEqual("acme-ltd", job.CompanyCanonicalName);
        Assert.AreEqual("it-jobs", job.CategoryTag);
        Assert.AreEqual("IT Jobs", job.CategoryLabel);
        Assert.AreEqual(51.5074m, job.Latitude);
        Assert.AreEqual(-0.1278m, job.Longitude);
        Assert.AreEqual("[\"UK\",\"London\",\"Central London\",\"The City\"]", job.LocationAreaJson);

        using var payload = JsonDocument.Parse(job.RawPayloadJson);
        Assert.AreEqual(70000m, payload.RootElement.GetProperty("salary_min").GetDecimal());
        Assert.AreEqual(90000m, payload.RootElement.GetProperty("salary_max").GetDecimal());
    }
}
