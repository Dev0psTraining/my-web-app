// HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DevOpsDemo.Models;
using DevOpsDemo.Services;

namespace DevOpsDemo.Controllers;

public class HomeController : Controller
{
    private readonly IMessageService _messageService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    public IActionResult Index()
    {
        ViewBag.Message = _messageService.GetWelcomeMessage();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Feedback()
    {
        return View(new FeedbackModel());
    }

    [HttpPost]
    public IActionResult Feedback(FeedbackModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        ViewBag.SuccessMessage = "Thank you for your feedback!";
        return View("Confirmation", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

// Models
namespace DevOpsDemo.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class FeedbackModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 1000 characters")]
        public string Message { get; set; } = string.Empty;
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; } = 3;
    }
}

// Services
namespace DevOpsDemo.Services
{
    public interface IMessageService
    {
        string GetWelcomeMessage();
    }

    public class MessageService : IMessageService
    {
        public string GetWelcomeMessage()
        {
            return "Welcome to the DevOps Pipeline Demo!";
        }
    }
}