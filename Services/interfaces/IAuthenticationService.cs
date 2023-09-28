using Microsoft.AspNetCore.Mvc;

namespace ToyEcommerceASPNET.Services.interfaces;

using ToyEcommerceASPNET.Models;

public interface IAuthenticationService
{
    Task<IActionResult> SignUp([FromBody] SignUpRequest request);
    Task<IActionResult> SignIn([FromBody] SignInRequest request);
}

public class SignUpRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}

public class SignInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}