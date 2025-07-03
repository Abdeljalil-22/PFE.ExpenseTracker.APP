using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

public class McpEndToEndTests
{
    [Fact]
    public async Task MCP_Can_Create_Expense_From_Natural_Language()
    {
        using var client = new HttpClient();
        var request = new
        {
            prompt = "Add an expense of 20 dollars for coffee today",
            userId = "demo-user-guid"
        };
        var response = await client.PostAsJsonAsync("http://localhost:5100/api/mcp/process", request);
        var result = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, result);
        Assert.Contains("expense", result.ToLower());
    }
}
