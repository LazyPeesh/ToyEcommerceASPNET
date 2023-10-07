using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;
using BCrypt.Net;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models.interfaces;

namespace ToyEcommerceASPNET.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _config;

        public AuthenticationService(IOptions<DatabaseSettings> databaseSettings, IConfiguration config)
        {
            // Connect to MongoDB
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            // Get the database
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            // Get the user collection
            _users = mongoDatabase.GetCollection<User>(databaseSettings.Value.UserCollectionName);

            // Get configuration for JWT secret
            _config = config;
        }

        // Sign up a new user
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            try
            {
                // Check if fullname or email is empty
                if (string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Email))
                {
                    // If so, return an error response
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message = "Fullname and email are required"
                    });
                }

                // Check if email is in the correct format
                if (!Regex.IsMatch(request.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    // If not, return an error response
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message = "Email is not in the correct format"
                    });
                }

                // Check if fullname has at least 2 words and does not contain any special characters but spaces or numbers
                if (!Regex.IsMatch(request.FullName, @"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$"))
                {
                    // If not, return an error response
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message =
                            "Fullname must have at least 2 words and does not contain any special characters but spaces or numbers"
                    });
                }

                // Check if email is already in use
                var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
                if (user != null)
                {
                    // If so, return an error response
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message = "Email is already in use"
                    });
                }

                // Check if the requested password has at least 8 characters, 1 uppercase letter, 1 lowercase letter, and 1 number
                if (!Regex.IsMatch(request.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$"))
                {
                    // If not, return an error response
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message =
                            "Password must have at least 8 characters, 1 uppercase letter, 1 lowercase letter, and 1 number"
                    });
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create a new user
                user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Password = passwordHash
                };

                // Add the new user to the database
                await _users.InsertOneAsync(user);

                // Generate an access token
                var accessToken = GenerateAccessToken(user);

                // Return user info and token
                return new OkObjectResult(new
                {
                    User = user,
                    Token = new
                    {
                        Access_Token = accessToken,
                        Expires_In = "1d"
                    }
                });
            }
            catch (Exception e)
            {
                // Handle error and return appropriate response
                return new BadRequestObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }

        // Sign in an existing user
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            try
            {
                // Find the user by email
                var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    // If user not found or password incorrect, return an error response
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message = "Invalid credentials"
                    });
                }

                // Generate access token and refresh token
                var accessToken = GenerateAccessToken(user);

                // Return a success response with user info and tokens
                return new OkObjectResult(new
                {
                    user = user,
                    access_token = accessToken,
                    expires_in = "1d"
                });
            }
            catch (Exception e)
            {
                // Handle error and return appropriate response
                return new BadRequestObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }

        // Generate an access token for a user
        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "true" : "false")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}