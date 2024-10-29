using NBomber.Contracts;
using NBomber.CSharp;

namespace RateLimiter.Integration.Tests;

public class TokenBucketRateLimitTests
{
    private const string HelloUrl = "http://localhost:5002/ServiceB/hello";
    private const string PingUrl = "http://localhost:5002/ServiceB/ping";
    
    [Fact]
    public async Task ShouldReturnCorrectSuccessAndErrorCounts()
    {
        // Define the HTTP client
        var client = new HttpClient();
        // var scenario = CreateTokenBucketRateLimitScenario(HelloUrl, "token_bucket_burst_test", client, 110);
        var scenario = Scenario.Create("token_bucket_burst_test", async context =>
            {
                var response = await client.GetAsync(HelloUrl);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    ? Response.Ok()
                    : Response.Fail(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 10, during: TimeSpan.FromSeconds(1), interval: TimeSpan.FromSeconds(1))
            )
            .WithoutWarmUp();
        
        // Run the scenario
        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        // Verify the outcome
        var successCount = result.ScenarioStats[0].AllOkCount;
        var errorCount = result.ScenarioStats[0].AllFailCount;

        // Assert that 110 requests were successful
        Assert.Equal(10, successCount);

        // Assert that there were no errors
        Assert.Equal(0, errorCount);
        
        // simulate 10s delay here
        await Task.Delay(TimeSpan.FromSeconds(10));
        
        var scenarioAfter = Scenario.Create("token_bucket_burst_test", async context =>
            {
                var response = await client.GetAsync(HelloUrl);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    ? Response.Ok()
                    : Response.Fail(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 100, during: TimeSpan.FromSeconds(1), interval: TimeSpan.FromSeconds(1))
            )
            .WithoutWarmUp();
        
        // Run the scenario
        result = NBomberRunner
            .RegisterScenarios(scenarioAfter)
            .Run();

        // Verify the outcome
        successCount = result.ScenarioStats[0].AllOkCount;
        errorCount = result.ScenarioStats[0].AllFailCount;

        // Assert that 110 requests were successful
        Assert.Equal(100, successCount);

        // Assert that there were no errors
        Assert.Equal(0, errorCount);
    }
    
    [Fact]
    public async Task ShouldReturnCorrectSuccessAndErrorCountsForOverLimitSenario()
    {
        // Define the HTTP client
        var client = new HttpClient();
        // var scenario = CreateTokenBucketRateLimitScenario(HelloUrl, "token_bucket_burst_test", client, 110);
        var scenario = Scenario.Create("token_bucket_burst_test", async context =>
            {
                var response = await client.GetAsync(HelloUrl);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    ? Response.Ok()
                    : Response.Fail(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 10, during: TimeSpan.FromSeconds(1), interval: TimeSpan.FromSeconds(1))
            )
            .WithoutWarmUp();
        
        // Run the scenario
        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        // Verify the outcome
        var successCount = result.ScenarioStats[0].AllOkCount;
        var errorCount = result.ScenarioStats[0].AllFailCount;

        // Assert that 110 requests were successful
        Assert.Equal(10, successCount);

        // Assert that there were no errors
        Assert.Equal(0, errorCount);
        
        // simulate 10s delay here
        await Task.Delay(TimeSpan.FromSeconds(10));
        
        var scenarioAfter = Scenario.Create("token_bucket_burst_test", async context =>
            {
                var response = await client.GetAsync(HelloUrl);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    ? Response.Ok()
                    : Response.Fail(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 101, during: TimeSpan.FromSeconds(1), interval: TimeSpan.FromSeconds(1))
            )
            .WithoutWarmUp();
        
        // Run the scenario
        result = NBomberRunner
            .RegisterScenarios(scenarioAfter)
            .Run();

        // Verify the outcome
        successCount = result.ScenarioStats[0].AllOkCount;
        errorCount = result.ScenarioStats[0].AllFailCount;

        // Assert that 110 requests were successful
        Assert.Equal(100, successCount);

        // Assert that there were no errors
        Assert.Equal(1, errorCount);
    }
    
    [Theory]
    [InlineData(40, 40)]
    public void ShouldFollowAttributePolicyInsteadOfGlobal(int totalRequests, int expectedSuccess)
    {
        var client = new HttpClient();
        // Create the step for your scenario
        var scenario = CreateTokenBucketRateLimitScenario(PingUrl, "rate_limiter_test", client, totalRequests);
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
    
    private static ScenarioProps CreateTokenBucketRateLimitScenario(string url, string name, HttpClient client, int totalRequests)
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