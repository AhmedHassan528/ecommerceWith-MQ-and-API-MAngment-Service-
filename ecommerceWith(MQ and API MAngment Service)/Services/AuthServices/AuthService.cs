
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication_With_JWT.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authentication_With_JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendMail _sendMail;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<AppUser> usMan, ISendMail sendMail, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _sendMail = sendMail;
            _userManager = usMan;
            _roleManager = roleManager;
            _jwt = jwt.Value;

        }



        public async Task<AuthModel> RegisterAsync(RegisterModel model, string? ReqUrl)
        {

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = "Username is already taken!" };

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email.ToLower(),
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new AuthModel
                {
                    Message = "Error: " + string.Join(" | ", result.Errors.Select(e => e.Description)),
                    IsAuthenticated = false
                };
            }

            // generate confirm token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send Email
            var EmailSend = await _sendMail.SendEmailAsync(model.Email, "Confirmation Your Account", token, "ConfirmEmail", ReqUrl);
            if (!string.IsNullOrEmpty(EmailSend))
            {
                await _userManager.DeleteAsync(user);
                return new AuthModel { Message = "something error when sending email" };
            }


            if (!result.Succeeded)
            {
                var Errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    Errors += error.Description + Environment.NewLine;
                }
                return new AuthModel { Message = Errors };
            }
            var roleName = "User";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                Roles = new List<string> { "User" }
            };
        }


        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel { Message = "Email or password is incorrect" };
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AuthModel { Message = "Must Confirm your email address" };
            }
            await _userManager.UpdateAsync(user);

            var jwtSecurityToken = await CreateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                Roles = roles.ToList()
            };

        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || user.Id != model.Userid)
                return "User id or email are incorrect";

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return "Role does not exist";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already has this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (!result.Succeeded)
                return "Role did not add Some thing error";

            return string.Empty;
        }


        public async Task<string> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return "Email not confirmed";
            }
            else
            {
                return string.Empty;
            }

        }

        public async Task<string> ForgotPasswordConfermationModel(ForgotPasswordConfermationModel model)
        {
            var user = await _userManager.FindByIdAsync(model.userId);
            if (model.userId == null || model.token == null)
            {
                return "Link expired";
            }
            else if (user == null)
            {
                return "User not Found";
            }
            model.token = model.token.Replace(" ", "+");
            var result = await _userManager.ResetPasswordAsync(user, model.token, model.confirmPassword);
            if (!result.Succeeded)
            {
                return "Password not reset";
            }
            return string.Empty;
        }

        public async Task<string> setAdminRole(string AdminID, string userEmail)
        {
            // var user = await _userManager.FindByEmailAsync(userEmail);

            // if (!await _roleManager.RoleExistsAsync("Admin"))
            // {
            //    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //    if (!roleResult.Succeeded)
            //    {
            //        throw new Exception("Failed to create Admin role");
            //    }
            // }

            // var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
            // if (!addToRoleResult.Succeeded)
            // {
            //    throw new Exception("Failed to add Admin");
            // }
            // return "User is now an Admin";


            try
            {
                var admin = await _userManager.FindByIdAsync(AdminID);



                var user = await _userManager.FindByEmailAsync(userEmail);

                if (admin == null || user == null)
                {
                    throw new Exception("Admin or user not found");
                }

                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception("Failed to create Admin role");
                    }
                }

                if (await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception("Failed to add Admin");
                    }
                    return "User is now an Admin";
                }
                else
                {
                    throw new Exception("Some thing Error!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error!: " + ex.Message);
            }


        }


        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("CustomerName", $"{user.FirstName} {user.LastName}"),
        new Claim("uid", user.Id)
    }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDay),
                signingCredentials: creds);

            return jwtSecurityToken;
        }


        public Task<string> DeleteAccount(string error, string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<UsersDto>> GetAllUsersAsync(string AdminID)
        {
            var admin = await _userManager.FindByIdAsync(AdminID);
            if (admin == null || !await isAdmin(AdminID))
            {
                return new List<UsersDto>();
            }

            var users = _userManager.Users.ToList();
            var usersDto = new List<UsersDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersDto.Add(new UsersDto
                {
                    ID = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    email = user.Email,
                    UserRoles = roles.ToList(),
                    PhoneNumber = user.PhoneNumber
                });
            }

            return usersDto;
        }

        public async Task<bool> isAdmin(string userId)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!roleResult.Succeeded)
                {
                    return false;
                }
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            else if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> isUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
