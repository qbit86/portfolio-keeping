using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Diversifolio.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
internal sealed class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;

    internal ErrorModel(ILogger<ErrorModel> logger) => _logger = logger;

    internal string? RequestId { get; private set; }

    internal bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    internal void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
}
