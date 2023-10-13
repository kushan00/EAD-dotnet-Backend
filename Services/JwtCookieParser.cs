using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace backend.Services;

public class JwtCookieParser
{
    private readonly RequestDelegate _next;
    private readonly AccountService _accountService;

    public JwtCookieParser(AccountService accountService, RequestDelegate next)
    {
        _accountService = accountService;
        _next = next;
    }

    /// <summary>
    /// This function reads a JWT token from a cookie, extracts the account ID from the token, retrieves
    /// user details from a database using the account ID, and stores the user details in the HttpContext
    /// for use in downstream middleware or controllers.
    /// </summary>
    /// <param name="HttpContext">The HttpContext object represents the current HTTP request and response.
    /// It contains information about the incoming request, such as headers, cookies, and query parameters,
    /// as well as methods to manipulate the response.</param>

    public async Task InvokeAsync(HttpContext context)
    {
        var jwtCookie = context.Request.Cookies["Train"];

        if (!string.IsNullOrEmpty(jwtCookie))
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtCookie);

            if (token != null)
            {
                // Extract the account ID from the token
                var accountId = token.Claims.FirstOrDefault(claim => claim.Type == "account_id")?.Value;

                if (!string.IsNullOrEmpty(accountId))
                {
                    // Use the account ID to retrieve user details from your database
                    var account = await _accountService.GetAccountAsync(accountId);
                    // Store user details in the HttpContext for use in downstream middleware or controllers

                    context.Items["UserDetails"] = account;
                }
            }
        }

        await _next(context);
    }
}
