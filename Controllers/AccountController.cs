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
    public async Task<ActionResult<List<AccountGetDTO>>> Get()
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office")
        {
            return Unauthorized();
        }
        var accounts = await _accountService.GetAccountAsync();

        // Convert the Account objects to AccountDTO objects
        var AccountGetDTOs = accounts.Select(account => new AccountGetDTO
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
        }).ToList();

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
        if (account.IsActive == false)
        {
            return BadRequest("Account is deactivated");
        }
        if (VerifyPasswordHash(loginDTO.Password!, account.Password!, account.Salt!))
        {
            string token = CreateToken(account);
            string result = "{\"token\" : \"" + token + "\" ,\"role\": \""
                    + account.UserRole + "\",\"id\" :\"" + account.Id + "\"}";

            // Set a cookie with the token
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(12), // Cookie expires in 1 hour
                HttpOnly = true,   // Makes the cookie accessible only through HTTP requests, not JavaScript
                Secure = true,     // Sends the cookie only over HTTPS if available
                SameSite = SameSiteMode.None // Restricts cookie sharing between sites
            };

            // Set the cookie in the response with a name
            Response.Cookies.Append("Train", token, cookieOptions);

            return Ok(result);
        }
        return BadRequest("Invalid credentials");
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
        account.IsActive = true;
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

        account.Name = accountPutDTO.Name ?? account.Name;
        account.Address = accountPutDTO.Address ?? account.Address;
        account.NIC = accountPutDTO.NIC ?? account.NIC;
        account.Number = accountPutDTO.Number ?? account.Number;
        account.Email = accountPutDTO.Email ?? account.Email;
        account.DOB = accountPutDTO.DOB ?? account.DOB;
        account.Gender = accountPutDTO.Gender ?? account.Gender;
        account.UserRole = accountPutDTO.UserRole ?? account.UserRole;

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
    public async Task<IActionResult> UserStatus(string id, AccountStatusDTO accountStatusDTO)
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office" && accountStatusDTO.IsActive != false)
        {
            return Unauthorized();
        }

        var account = await _accountService.GetAccountAsync(id);

        if (account is null)
        {
            return NotFound();
        }
        account.IsActive = accountStatusDTO.IsActive;

        await _accountService.UpdateAccountAsync(id, account);

        return Ok();
    }

    [HttpGet("Profile")]
    public ActionResult<AccountGetDTO> GetProfile()
    {
        var jwtCookie = HttpContext.Request.Cookies["Train"];
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount)
        {
            System.Console.WriteLine(accountObj);
            if (LogUserAccount is null)
            {
                return NotFound();
            }
            // Convert the Account objects to AccountDTO objects
            var AccountGetDTO = new AccountGetDTO
            {
                Id = LogUserAccount.Id,
                Name = LogUserAccount.Name,
                NIC = LogUserAccount.NIC,
                Address = LogUserAccount.Address,
                Number = LogUserAccount.Number,
                Email = LogUserAccount.Email,
                DOB = LogUserAccount.DOB,
                Gender = LogUserAccount.Gender,
                IsActive = LogUserAccount.IsActive,
                UserRole = LogUserAccount.UserRole,
                CreatedTime = LogUserAccount.CreatedTime
            };

            return AccountGetDTO;

        }
        return NotFound();
    }

    [HttpDelete("Profile")]
    public async Task<IActionResult> DeleteProfile()
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.Id != null)
        {
            await _accountService.RemoveAccountAsync(LogUserAccount.Id);

            return Ok();
        }
        return NotFound();
    }

    [HttpPut("Profile")]
    public async Task<IActionResult> UpdateProfile(AccountPostDTO accountPutDTO)
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.Id != null)
        {
            if (LogUserAccount is null)
            {
                return NotFound();
            }

            if (accountPutDTO.Password is not null)
            {
                CreatePasswordHash(accountPutDTO.Password!, out byte[] passwordHash, out byte[] passwordSalt);
                LogUserAccount.Password = passwordHash;
                LogUserAccount.Salt = passwordSalt;
            }

            LogUserAccount.Name = accountPutDTO.Name ?? LogUserAccount.Name;
            LogUserAccount.Address = accountPutDTO.Address ?? LogUserAccount.Address;
            LogUserAccount.NIC = accountPutDTO.NIC ?? LogUserAccount.NIC;
            LogUserAccount.Number = accountPutDTO.Number ?? LogUserAccount.Number;
            LogUserAccount.Email = accountPutDTO.Email ?? LogUserAccount.Email;
            LogUserAccount.DOB = accountPutDTO.DOB ?? LogUserAccount.DOB;
            LogUserAccount.Gender = accountPutDTO.Gender ?? LogUserAccount.Gender;
            LogUserAccount.UserRole = accountPutDTO.UserRole ?? LogUserAccount.UserRole;

            await _accountService.UpdateAccountAsync(LogUserAccount.Id, LogUserAccount);

            return Ok();
        }
        return NoContent();
    }

    [HttpPut("Profile/Status")]
    public async Task<IActionResult> UserStatusProfile()
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.Id != null)
        {
            if (LogUserAccount is null)
            {
                return NotFound();
            }
            LogUserAccount.IsActive = false;

            await _accountService.UpdateAccountAsync(LogUserAccount.Id, LogUserAccount);
            return Ok();
        }
        return NoContent();
    }
}