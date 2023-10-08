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
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly JwtSettings jwtSettings;
    public AccountController(AccountService accountService, IOptions<JwtSettings> options)
    {
        _accountService = accountService;
        jwtSettings = options.Value;
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

    }
    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
    private string CreateToken(Account account)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.SecurityKey);
        var tokenDesc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] { new("NIC", account.NIC!), new(ClaimTypes.Role, account.UserRole!), new("account_id", account.Id!) }),
            Expires = DateTime.Now.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDesc);
        string jwt = tokenHandler.WriteToken(token);
        return jwt;
    }


    [HttpGet]
    public async Task<List<AccountGetDTO>> Get()
    {
        var accounts = await _accountService.GetAccountAsync();

        // Convert the Account objects to AccountDTO objects
        var AccountGetDTOs = accounts.Select(account => new AccountGetDTO
        {
            Name = account.Name,
            NIC = account.NIC,
            Address = account.Address,
            Number = account.Number,
            Email = account.Email,
            DOB = account.DOB,
            Gender = account.Gender,
            IsActive = account.IsActive,
            UserRole = account.UserRole,
        }).ToList();

        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountIdObj))
        {
            // Now, you can use the accountId in your controller logic
            // Example: var userDetails = userRepository.GetUserDetails(accountId);

            Console.WriteLine($"Account IDssssss: {accountIdObj}");
        }


        return AccountGetDTOs;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<AccountGetDTO>> Get(string id)
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office")
        {
            return Unauthorized();
        }

        var account = await _accountService.GetAccountAsync(id);
        if (account is null)
        {
            return NotFound();
        }
        // Convert the Account objects to AccountDTO objects
        var AccountGetDTO = new AccountGetDTO
        {
            Id = account.Id,
            Name = account.Name,
            NIC = account.NIC,
            Address = account.Address,
            Number = account.Number,
            Email = account.Email,
            DOB = account.DOB,
            Gender = account.Gender,
            IsActive = account.IsActive,
            UserRole = account.UserRole,
            CreatedTime = account.CreatedTime
        };
        Console.WriteLine($"Account ID: ");

        return AccountGetDTO;
    }

    //GET Account => Login
    [HttpPost("login")]
    public async Task<ActionResult> Get([FromBody] LoginDTO loginDTO)
    {

        var account = await _accountService.GetAccountLogin(loginDTO.NIC!);
        if (account is null)
        {
            return BadRequest("Account not found");
        }
        else
        {
            if (VerifyPasswordHash(loginDTO.Password!, account.Password!, account.Salt!))
            {
                string token = CreateToken(account);
                string result = "{\"token\" : \"" + token + "\" ,\"role\": \""
                        + account.UserRole + "\"}";

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
    public async Task<IActionResult> Post(AccountPostDTO accountPostDTO)
    {
        var accountCheck = await _accountService.GetAccountLogin(accountPostDTO.NIC!);
        if (accountCheck is not null)
        {
            return BadRequest("NIC already available");
        }
        Account account = new();
        CreatePasswordHash(accountPostDTO.Password!, out byte[] passwordHash, out byte[] passwordSalt);
        account.Name = accountPostDTO.Name;
        account.Address = accountPostDTO.Address;
        account.NIC = accountPostDTO.NIC;
        account.Number = accountPostDTO.Number;
        account.Email = accountPostDTO.Email;
        account.Password = passwordHash;
        account.Salt = passwordSalt;
        account.DOB = accountPostDTO.DOB;
        account.Gender = accountPostDTO.Gender;
        account.IsActive = accountPostDTO.IsActive;
        account.UserRole = accountPostDTO.UserRole;
        await _accountService.CreateAccountAsync(account);

        return CreatedAtAction(nameof(Get), new { id = account.Id }, account);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, AccountPostDTO accountPutDTO)
    {
        var account = await _accountService.GetAccountAsync(id);

        if (account is null)
        {
            return NotFound();
        }

        if (accountPutDTO.Password is not null)
        {
            CreatePasswordHash(accountPutDTO.Password!, out byte[] passwordHash, out byte[] passwordSalt);
            account.Password = passwordHash;
            account.Salt = passwordSalt;
        }

        account.Name = accountPutDTO.Name;
        account.Address = accountPutDTO.Address;
        account.NIC = accountPutDTO.NIC;
        account.Number = accountPutDTO.Number;
        account.Email = accountPutDTO.Email;
        account.DOB = accountPutDTO.DOB;
        account.Gender = accountPutDTO.Gender;
        account.IsActive = accountPutDTO.IsActive;
        account.UserRole = accountPutDTO.UserRole;

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

    [HttpPut("Status/{id:length(24)}")]
    public async Task<IActionResult> UserStatus(string id, AccountPostDTO accountPutDTO)
    {
        var account = await _accountService.GetAccountAsync(id);

        if (account is null)
        {
            return NotFound();
        }
        account.IsActive = accountPutDTO.IsActive;

        await _accountService.UpdateAccountAsync(id, account);

        return Ok();
    }

}
