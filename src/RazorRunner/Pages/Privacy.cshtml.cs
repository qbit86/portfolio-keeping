using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Diversifolio.Pages;

internal sealed class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    internal PrivacyModel(ILogger<PrivacyModel> logger) => _logger = logger;
}
