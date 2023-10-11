using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/v1/user")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}
		// GET: api/<UserController>
		[HttpGet]
		public async Task<IActionResult> GetUsers([FromQuery] int page = 1)
		{
			try
			{
				// Adjust page size as needed
				int pageSize = 10;

				// Count total users
				long totalUsers =  await _userService.CountUsersAsync();

				// Calculate total pages
				int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

				if (page < 1 || page > totalPages)
				{
					return new BadRequestObjectResult(new
					{
						status = "error",
						Message = "Invalid page number"
					});
				}

				// Get users for the specified page
				var users =  _userService.GetUsers(page, pageSize);

				if (users == null)
				{
					return new OkObjectResult(new
					{
						status = "success",
						users = Enumerable.Empty<User>(),
						totalPage = 0,
						totalLength = 0
					});
				}

				return new OkObjectResult(new
				{
					status = "success",
					user = users,
					totalPage = totalPages,
					totalLength = totalUsers
				});
			}
			catch (Exception e) { 
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
		
		}

		// GET api/<UserController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById( [FromRoute] string id)
		{
			try
			{
				var user = _userService.GetUserById(id);
				if (user == null)
					return new BadRequestObjectResult(new
					{

						status = "error",
						Message = "User not found"
					});

				return new OkObjectResult( new { 
					status = "success",
					Message = "User found",
					user });
			}
			catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
		}

		// POST api/<UserController>
		[HttpPost]
		public async Task<IActionResult> Post(string fullname, string email, string password)
		{
			try
			{
				var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
				var user = new User
				{
					FullName = fullname,
					Email = email,
					Password = passwordHash // Store the hashed password
				};

				_userService.CreateUser(user);

				return new OkObjectResult(new
				{
					status = "success",
					user = user
				});
			}catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
			
		}

		// PUT api/<UserController>/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put([FromRoute] string id, [FromBody] User user)
		{
			try
			{
				var existingUser = _userService.GetUserById(id);

				if (existingUser == null)
				  return new BadRequestObjectResult(new
				 {

					 status = "error",
					 Message = "User not found"
				 });


				_userService.UpdateUser(id, user);
				return new ObjectResult(new
				{
					status = "success",
					message = "User was updated",
					user = user
				});
			}
			catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
		
		}

		// DELETE api/<UserController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			try
			{
				var existingUser = _userService.GetUserById(id);

				if (existingUser == null)
					return new BadRequestObjectResult(new
					{

						status = "error",
						Message = "User not found"
					});

				_userService.RemoveUser(id);
				return new OkObjectResult(new
				{
					Message = "User was deleted"
				});
			}
			catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
			
		}
	}
}
