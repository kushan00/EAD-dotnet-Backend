using backend.Models;
using backend.DTO;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TrainController : ControllerBase
{
    private readonly TrainController _trainController;
    public TrainController(TrainController trainController)
    {
        _trainController = trainController;
    }

    [HttpGet]
    public async Task<List<Account>> Get() => await _trainService.GetAccountAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Account>> Get(string id)
    {
        var account = await _accountService.GetAccountAsync(id);
        if (account is null)
        {
            return NotFound();
        }
        return account;
    }

    //GET Account => Login
    [HttpPost("login")]
    public async Task<ActionResult> Get([FromBody] AccountDTO accountDTO)
    {

        var account = await _accountService.GetAccountLogin(accountDTO.NIC!);
        if (account is null)
        {
            return BadRequest("Account not found");
        }
        else
        {
            if (VerifyPasswordHash(accountDTO.Password!, account.Password!, account.Salt!))
            {
                string token = CreateToken(account);
                string result = "{\"token\" : \"" + token + "\"}";

                // Set a cookie with the token
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(12), // Cookie expires in 1 hour
                    HttpOnly = true,   // Makes the cookie accessible only through HTTP requests, not JavaScript
                    Secure = true,     // Sends the cookie only over HTTPS if available
                    SameSite = SameSiteMode.Strict // Restricts cookie sharing between sites
                };

                // Set the cookie in the response with a name
                Response.Cookies.Append("Train", token, cookieOptions);

                return Ok(result);

            }
            return NotFound();
        }

    }

    [HttpPost]
    public async Task<IActionResult> Post(AccountDTO accountDTO)
    {
        var accountCheck = await _accountService.GetAccountLogin(accountDTO.NIC!);
        if (accountCheck is not null)
        {
            return BadRequest("NIC already available");
        }
        Account account = new();
        CreatePasswordHash(accountDTO.Password!, out byte[] passwordHash, out byte[] passwordSalt);
        account.Name = accountDTO.Name;
        account.Address = accountDTO.Address;
        account.NIC = accountDTO.NIC;
        account.Number = accountDTO.Number;
        account.Email = accountDTO.Email;
        account.Password = passwordHash;
        account.Salt = passwordSalt;
        account.DOB = accountDTO.DOB;
        account.Gender = accountDTO.Gender;
        account.IsActive = accountDTO.IsActive;
        account.UserRole = accountDTO.UserRole;
        await _accountService.CreateAccountAsync(account);

        return CreatedAtAction(nameof(Get), new { id = account.Id }, account);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, AccountDTO accountUpdate)
    {
        var account = await _accountService.GetAccountAsync(id);

        if (account is null)
        {
            return NotFound();
        }

        if (accountUpdate.Password is not null)
        {
            CreatePasswordHash(accountUpdate.Password!, out byte[] passwordHash, out byte[] passwordSalt);
            account.Password = passwordHash;
            account.Salt = passwordSalt;
        }

        account.Name = accountUpdate.Name;
        account.Address = accountUpdate.Address;
        account.NIC = accountUpdate.NIC;
        account.Number = accountUpdate.Number;
        account.Email = accountUpdate.Email;
        account.DOB = accountUpdate.DOB;
        account.Gender = accountUpdate.Gender;
        account.IsActive = accountUpdate.IsActive;
        account.UserRole = accountUpdate.UserRole;

        await _accountService.UpdateAccountAsync(id, account);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var User = await _accountService.GetAccountAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        await _accountService.RemoveAccountAsync(id);

        return Ok();
    }

}
