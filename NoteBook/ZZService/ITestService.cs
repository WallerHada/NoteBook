
using System.Threading.Tasks;

namespace NoteBook.ZZService;
public interface ITestService
{
    Task<string> OnGet(string url);
}
