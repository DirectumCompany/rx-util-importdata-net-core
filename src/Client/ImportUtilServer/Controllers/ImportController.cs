using ImportUtilServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImportUtilServer.Controllers;

[Route("/api")]
public class ImportController : Controller
{
    private readonly Import _import;
    
    public ImportController(Import import) : base()
    {
        _import = import;
    }
    
    [HttpGet("import")]
    public IActionResult Import(string arguments)
    {
        _import.Execute(arguments);
        return Ok();
    }

    [HttpGet("isactive")]
    public ActionResult<bool> IsActive()
    {
        return _import.IsActive();
    }

    [HttpGet("getoutputdata")]
    public ActionResult<IEnumerable<string>> GetOutputData()
    {
        var result = _import.GetOutputData();
        _import.ClearOutputData();
        return Ok(result);
    }
}