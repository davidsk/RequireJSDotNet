using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClassLibrary1.EntryPointResolver
{
    public interface IEntryPointResolver
    {
        string Resolve(ViewContext viewContext, string baseUrl, string entryPointRoot);
    }
}
