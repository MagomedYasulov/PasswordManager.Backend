using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using PasswordManager.Backend.Controllers;
using PasswordManager.Backend.Data.Entities;
using PasswordManager.Backend.Data;
using PasswordManager.Backend.DTOs;
using System.Diagnostics;

namespace PasswordManager.Backend.Models
{
    public class JwtBearerEventsImplimintation
    {
        public static async Task OnChallenge(JwtBearerChallengeContext context)
        {
            context.HandleResponse();
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
            var serializeSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;

            var problemDetails = new ProblemDetails
            {
                Type = options.Value.ClientErrorMapping[401].Link,
                Title = context.AuthenticateFailure != null ? localizer["InvalidToken"] : localizer["TokenNotFound"],
                Status = StatusCodes.Status401Unauthorized,
                Instance = context.Request.Path,
            };
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            problemDetails.Detail = !string.IsNullOrEmpty(context.ErrorDescription) ? localizer["InvalidTokenDetail", context.ErrorDescription] : problemDetails.Title;

            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.HttpContext.Response.ContentType = "application/problem+json";

            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, serializeSettings));
        }

        public static async Task OnForbidden(ForbiddenContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
            var serializeSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;

            var problemDetails = new ProblemDetails
            {
                Type = options.Value.ClientErrorMapping[403].Link,
                Title = localizer["Forbidden"],
                Status = StatusCodes.Status403Forbidden,
                Instance = context.Request.Path,
                Detail = localizer["Forbidden"],
            };
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);

            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.HttpContext.Response.ContentType = "application/problem+json";

            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, serializeSettings));
        }

        public static Task OnTokenValidated(TokenValidatedContext context)
        {
            var userPrincipal = context.Principal;
            UserDTO? userDTO = null;
            string? accessTokenJti = null;
            try
            {
                userDTO = JsonConvert.DeserializeObject<UserDTO>(userPrincipal?.FindFirst("user")?.Value ?? "");
                accessTokenJti = userPrincipal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value.ToString();
            }
            catch { }

            var repository = context.HttpContext.RequestServices.GetRequiredService<IRepository>();

            if (string.IsNullOrWhiteSpace(accessTokenJti) || userDTO == null
                || !repository.Any<Token>(t => t.UserId == userDTO.Id && t.AccessTokenJti == accessTokenJti)) //userDTO == null || !isExist
            {
                var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
                context.Fail(localizer["InvalidAccessToken"]);
                return Task.CompletedTask;
            }
            var token = context.SecurityToken as JsonWebToken;

            context.HttpContext.Items.Add("user", userDTO);
            context.HttpContext.Items.Add("login", userDTO.Login);
            context.HttpContext.Items.Add("access_token", token!.EncodedToken);
            context.HttpContext.Items.Add("jti", accessTokenJti); //token.Id

            return Task.CompletedTask;
        }

        public static Task OnMessageReceived(MessageReceivedContext context)
        {
            var path = context.HttpContext.Request.Path;
            if (!IsQueryOrCookiePath(path))
                return Task.CompletedTask;

            var accessTokenQuery = context.Request.Query["access_token"];
            var accessTokenHeaders = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(accessTokenQuery))
                context.Token = accessTokenQuery;
            else if (!string.IsNullOrEmpty(accessTokenHeaders))
                context.Token = accessTokenHeaders;

            return Task.CompletedTask;
        }

        private static bool IsQueryOrCookiePath(PathString path)
        {
            return false;
            //foreach (var hub in hubs)
            //    if (path.StartsWithSegments(hub)) return true;
            //return false;
        }
    }
}

