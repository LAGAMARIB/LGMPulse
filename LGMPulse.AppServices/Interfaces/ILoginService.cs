using LGMDomains.Common;
using LGMDomains.Identity;

namespace LGMPulse.AppServices.Interfaces;

public interface ILoginService
{
    Task<LGMResult<string>> ChangePasswordAsync(LoginModel model);
    Task<LGMResult<string>> RecoverPasswordAsync(LoginModel loginModel);
    Task<LGMResult<LGMUser>> ValidateLoginAsync(LoginModel loginModel);
    Task<LGMResult<LGMUser>> CreateUser(RequestRegisterModel model);
}
