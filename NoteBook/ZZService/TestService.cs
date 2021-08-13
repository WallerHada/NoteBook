
using NoteBook.HttpNote;

namespace NoteBook.ZZService;
public class TestService : ITestService
{
    private readonly BaseClients _baseClients;

    public TestService(BaseClients baseClients)
    {
        _baseClients = baseClients;
    }

    public Task<string> OnGet(string url)
    {
        return _baseClients.OnGet(url);
    }
}
