using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.InterFaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]// account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExist(registerDTO.Username)) return BadRequest("username is taken");
        return Ok();
        /* using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserNmae = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new UserDTO
        {
            Username = user.UserNmae,
            Token = tokenService.CreateToken(user)
        }; */
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> login(LoginDTO loginDTO)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());
        if(user == null) return Unauthorized("invaild username");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < ComputedHash.Length; i++)
        {
            if(ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("invaild password");
        }
        return new UserDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }
    private async Task<bool> UserExist(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
