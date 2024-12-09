using System.Linq;
using BCrypt.Net;
using System;
using E_Commerce.Models;
using E_Commerce.DTOs;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class UserServices
    {
        private readonly E_CommerceDB _context;
        private const int WORK_FACTOR = 11;


        public UserServices(E_CommerceDB context)
        {
            _context = context;
        }

        public async Task<AppUser> AddUser(UserDTO user)
        {
            var userEntity = new AppUser
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                IsAdmin = false

            };
            userEntity.Password = HashPassword(user.Password);
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            return userEntity;

        }

        public async Task<AppUser> GetUserByUsername(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }
        public async Task<IEnumerable<AppUser>> GettAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<AppUser> GetUserById(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }

        
        public async Task<bool> UserExists(string name)
        {
            return await _context.Users.AnyAsync(u => u.Name == name);
        }

        public bool VerifyPassword(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == username);
            if (user != null)
            {
                return ValidatePassword(password, user.Password);
            }
            return false;
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WORK_FACTOR);
        }

        private bool ValidatePassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
