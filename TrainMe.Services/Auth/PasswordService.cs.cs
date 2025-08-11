using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainMe.Services.Auth
{
    public class PasswordService
    {
        // Hash password
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password is required");
            }
            // hash password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify password
        public bool VerifyPassword(string password, string hash)
        {
            if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            {
                return false;
            }
            // verify password using BCrypt
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
