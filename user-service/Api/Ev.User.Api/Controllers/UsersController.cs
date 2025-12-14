using Microsoft.AspNetCore.Mvc;

namespace Ev.User.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<UserDto> Users = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(Users);

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateUserRequest request)
    {
        var user = new UserDto(Guid.NewGuid(), request.Email, request.FullName);
        Users.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}

public record CreateUserRequest(string Email, string FullName);
public record UserDto(Guid Id, string Email, string FullName);
