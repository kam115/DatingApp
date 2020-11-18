using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContxt _contxt;
        public AuthRepository(DataContxt contxt)
        {
            _contxt = contxt;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _contxt.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
            return null;


            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
                using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
                {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i <computedHash.Length; i++){
                    if(computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _contxt.Users.AddAsync(user);
            await _contxt.SaveChangesAsync();

            return user;

            
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _contxt.Users.AnyAsync(x => x.Username == username))
            return true;

            return false;
        }

        public Task Login(object username, object password)
        {
            throw new NotImplementedException();
        }
    }
}