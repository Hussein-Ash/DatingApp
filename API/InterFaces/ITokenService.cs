using System;
using API.Entities;

namespace API.InterFaces;

public interface ITokenService
{
    string CreateToken(AppUser user);

}
