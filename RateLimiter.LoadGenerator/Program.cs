// See https://aka.ms/new-console-template for more information

class program
{
    static async Task Main(string[] args)
    {
        var apiEndpoints = new List<string>
        {
            "http://localhost:5001/serviceA/hello",   // Service 1
            // "http://localhost:5002/api/echo",    // Service 2
            // "http://localhost:5003/api/ping"     // Service 3
        };

        int totalRequests = 1000;
        int concurrentUsers = 10;
        int requestsPerUser = totalRequests / concurrentUsers;

        var httpClient = new HttpClient();
        var tasks = new List<Task>();

        for (int i = 0; i < concurrentUsers; i++)
        {
            var userId = i + 1;
            tasks.Add(Task.Run(async () =>
            {
                for (int j = 0; j < requestsPerUser; j++)
                {
                    var endpoint = apiEndpoints[new Random().Next(apiEndpoints.Count)];
                    var response = await httpClient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"User {userId}: Success - {responseBody}");
                    }
                    else
                    {
                        Console.WriteLine($"User {userId}: Failed - {response.StatusCode}");
                    }

                    // Optional: Delay between requests to mimic realistic usage
                    await Task.Delay(100);
                }
            }));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("Load generation completed.");
    }
}