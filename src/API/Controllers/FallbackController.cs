using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace API.Controllers;

[AllowAnonymous]
public class FallbackController : Controller {
  // need hosting environment for base path
  public IWebHostEnvironment HostingEnv { get; }

  public FallbackController(IWebHostEnvironment env) {
    HostingEnv = env;
  }

  [HttpGet]
  public IActionResult Index() {
    return new PhysicalFileResult(Path.Combine(HostingEnv.WebRootPath, "index.html"),
        new MediaTypeHeaderValue("text/html")
    );

    //return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
  }
}