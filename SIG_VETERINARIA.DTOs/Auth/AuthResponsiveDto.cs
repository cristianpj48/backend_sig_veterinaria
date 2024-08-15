using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIG_VETERINARIA.DTOs.User;

namespace SIG_VETERINARIA.DTOs.Auth
{
    public class AuthResponsiveDto
    {
        public Boolean IsSuccess {  get; set; }
        public UserDetailResponsiveDto User {  get; set; }
        public string Token { get; set; }
    }

    public class TokenResponsiveDto
    {
        public string Token { get; set; }
    }
}
