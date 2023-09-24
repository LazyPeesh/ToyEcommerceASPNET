using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/[controller]")]
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
		public ActionResult<List<User>> Get()
		{
			return _userService.GetUsers();
		}

		// GET api/<UserController>/5
		[HttpGet("{id}")]
		public ActionResult<User> Get(string id)
		{
			var user = _userService.GetUserById(id);

			if (user == null)
				return NotFound($"Student with Id = {id} not found");

			return user;
		}

		// POST api/<UserController>
		[HttpPost]
		public ActionResult<User> Post([FromBody] User user)
		{
			_userService.CreateUser(user);

			return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
		}

		// PUT api/<UserController>/5
		[HttpPut("{id}")]
		public ActionResult Put(string id, [FromBody] User user)
		{
			var existingUser = _userService.GetUserById(id);

			if (existingUser == null)
				return NotFound($"User with Id = {id} not found");

			_userService.UpdateUser(id, user);
			return NoContent();
		}

		// DELETE api/<UserController>/5
		[HttpDelete("{id}")]
		public ActionResult Delete(string id)
		{
			var existingUser = _userService.GetUserById(id);

			if (existingUser == null)
				return NotFound($"User with Id = {id} not found");
			
			_userService.RemoveUser(id);
			return NoContent();
		}
	}
}
