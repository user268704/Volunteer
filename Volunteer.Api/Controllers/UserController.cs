using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volunteer.Api.Jwt;
using Volunteer.Models.Responses;
using Volunteer.Models.User;

namespace Volunteer.Api.Controllers;

/// <summary>
/// Контроллер для пользователей
/// </summary>
[ApiController]
[Route("user")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    private readonly UserManager<UserIdentity> _userManager;
    private readonly SignInManager<UserIdentity> _signInManager;
    private IJwtLogin _jwtLogin;
    private readonly IMapper _mapper;
    
    public UserController(UserManager<UserIdentity> userManager,
        SignInManager<UserIdentity> signInManager, 
        IMapper mapper, IJwtLogin jwtLogin)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _jwtLogin = jwtLogin;
    }
    
    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(UserLogin userLogin)
    {
        UserIdentity? user = await _userManager.FindByEmailAsync(userLogin.Email);

        if (user == null)
            return Ok(new ErrorResponse
            {
                Error = "user_not_found",
                Message = "Такого пользователя не существует"
            });

        bool isValidPassword = await _userManager.CheckPasswordAsync(user, userLogin.Password);

        if (isValidPassword)
        {
            await _signInManager.SignInAsync(user, true);
            string jwtToken = _jwtLogin.GetToken((List<Claim>)await _userManager.GetClaimsAsync(user));
            return Ok(jwtToken);
        }

        return Ok(new ErrorResponse
        {
            Error = "incorrect_data",
            Message = "Пароль или Email не верны"
        });
    }

    
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <returns></returns>
    [Route("register")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(UserRegister user)
    {
        UserIdentity fullUser = _mapper.Map<UserIdentity>(user);

        if (await _userManager.FindByEmailAsync(user.Email) != null)
            return Ok(new ErrorResponse
            {
                Error = "user_exists",
                Message = "Такой пользователь уже существует"
            });

        fullUser.UserName = fullUser.Email;
        var createResult = await _userManager.CreateAsync(fullUser, user.Password);
        if (createResult.Succeeded)
        {
            await _userManager.AddClaimsAsync(fullUser, new List<Claim>
            {
                new(ClaimTypes.Email, fullUser.Email),
                new("username", fullUser.UserName),
                new(ClaimTypes.Name, fullUser.Name),
                new(ClaimTypes.Role, "volunteer"),
                new(ClaimTypes.Surname, fullUser.Lastname),
                new(ClaimTypes.GivenName, fullUser.Patronymic),
            });
            
            await _userManager.AddToRoleAsync(fullUser, "volunteer");
            string jwtToken = _jwtLogin.GetToken((await _userManager.GetClaimsAsync(fullUser)).ToList());

            await _signInManager.SignInAsync(fullUser, true);
        
            return Ok(jwtToken);
        }

        return Ok(new ErrorResponse
        {
            Error = "create_user_error",
            Message = "Не удалось создать аккаунт"
        });
    }

    /// <summary>
    /// Выход из системы
    /// </summary>
    /// <returns></returns>
    [Route("signout")]
    [HttpPost]
    public IActionResult Signout()
    {
        _signInManager.SignOutAsync();

        return Ok();
    }

    /// <summary>
    /// Получить пользователя
    /// </summary>
    /// <returns></returns>
    [Route("get/{uid}")]
    [HttpGet]
    public async Task<IActionResult> Get(Guid uid)
    {
        UserIdentity? user = await _userManager.FindByIdAsync(uid.ToString());

        if (user == null)
            return Ok(new ErrorResponse
            {
                StatusCode = 404,
                Error = "Такого пользователя не существует"
            });

        return Ok(_mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Получить себя
    /// </summary>
    /// <returns></returns>
    [Route("get-me")]
    [HttpGet]
    public async Task<IActionResult> GetMe()
    {
        UserIdentity userMe =
            await _userManager.FindByEmailAsync(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email).Value);
        
        return Ok(_mapper.Map<UserDto>(userMe));
    }

    /// <summary>
    /// Изменить данные пользователя
    /// </summary>
    /// <returns></returns>
    [Route("edit")]
    [HttpPost]
    public IActionResult Edit()
    {
        throw new NotImplementedException();
    }
}