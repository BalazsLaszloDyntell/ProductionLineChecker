using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Simulation.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private Random _rnd = new Random();
    private int _minEntryDelayInMS = 50;
    private int _maxEntryDelayInMS = 5000;
    public List<Production> Productions { get; set; } = new List<Production>() {};
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
