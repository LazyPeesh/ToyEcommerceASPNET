using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Services;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        // POST api/v1/auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            // Print out the request
            Console.WriteLine(request);
            return await _authenticationService.SignUp(request);
        }

        // POST api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] SignInRequest request)
        {
            return await _authenticationService.SignIn(request);
        }
    }
}