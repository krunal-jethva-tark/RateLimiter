using NBomber.Contracts;
using NBomber.CSharp;

namespace RateLimiter.Integration.Tests;

public class FixedWindowRateLimitTests
{
    private const string EchoUrl = "http://localhost:5001/ServiceA/echo";
    private const string PingUrl = "http://localhost:5001/ServiceA/ping";
    
    [Theory]
    [InlineData("User1", 25, 20)] // exceeds limit
    [InlineData("User1", 20, 20)] // at limit
    [InlineData("User1", 15, 15)] // under limit
    [InlineData("User1", 30, 20)] // exceeds limit
    public void ShouldReturnCorrectSuccessAndErrorCounts(string userIdentity, int totalRequests, int expectedSuccess)
    {
        // Define the HTTP client
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Identity", userIdentity);

        // Create the step for your scenario
        var scenario = CreateFixedWindowRateLimitScenario(EchoUrl,"rate_limiter_test", client, totalRequests);
        
        // Run the scenario
        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        // Verify the outcome
        var successCount = result.ScenarioStats[0].AllOkCount;
        var errorCount = result.ScenarioStats[0].AllFailCount;
        
        // Assert that 20 requests were successful
        Assert.Equal(expectedSuccess, successCount);

        // Assert that 30 requests resulted in 429 errors
        Assert.Equal(totalRequests - expectedSuccess, errorCount);
    }

    [Fact]
    public void ShouldLimitRequestsPerUserInParallel()
    {
        const int totalUsers = 5;
        const int maxRequestsPerUser = 20;
        const int  totalRequests = 30; // Total requests for each user
        
        var scenarios = new ScenarioProps[totalUsers];
        for (var i = 0; i < totalUsers; i++)
        {
            var userIdentity = $"User{i + 1}";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Identity", userIdentity);

            scenarios[i] =
                CreateFixedWindowRateLimitScenario(EchoUrl, $"rate_limiter_test_{userIdentity}", client, totalRequests);
        }

        var result = NBomberRunner
            .RegisterScenarios(scenarios)
            .Run();
        
        foreach (var scenarioStat in result.ScenarioStats)
        {
            var successCount = scenarioStat.AllOkCount;
            var errorCount = scenarioStat.AllFailCount;

            // Assert that no user should exceed the allowed requests
            Assert.True(successCount <= maxRequestsPerUser, $"User exceeded request limit: {scenarioStat.ScenarioName}");

            // Assert that the error count equals totalRequests minus successCount
            Assert.Equal(totalRequests - successCount, errorCount);
        }

    }

    [Theory]
    [InlineData(20, 5)]
    public void ShouldFollowAttributePolicyInsteadOfGlobal(int totalRequests, int expectedSuccess)
    {
        var client = new HttpClient();
        // Create the step for your scenario
        var scenario = CreateFixedWindowRateLimitScenario(PingUrl, "rate_limiter_test", client, totalRequests);
        // Run the scenario
        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        // Verify the outcome
        var successCount = result.ScenarioStats[0].AllOkCount;
        var errorCount = result.ScenarioStats[0].AllFailCount;
        
        // Assert that 20 requests were successful
        Assert.Equal(expectedSuccess, successCount);

        // Assert that 30 requests resulted in 429 errors
        Assert.Equal(totalRequests - expectedSuccess, errorCount);
    }

    private static ScenarioProps CreateFixedWindowRateLimitScenario(string url, string name, HttpClient client, int totalRequests)
    {
        return Scenario.Create(name, async context =>
            {
                var response = await client.GetAsync(url);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    ? Response.Ok()
                    : Response.Fail(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: totalRequests, during: TimeSpan.FromSeconds(1), interval: TimeSpan.FromSeconds(1))
            )
            .WithoutWarmUp();
    }
}