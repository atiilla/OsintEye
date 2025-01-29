using System.Collections.Generic;
using System.Linq;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class MockAuthService
    {
        private List<User> _users;

        public MockAuthService()
        {
            _users = new List<User>
            {
                new User { Username = "test", Password = "test123", Email = "test@test.com" }
            };
        }

        public bool Login(string username, string password)
        {
            return _users.Any(u => u.Username == username && u.Password == password);
        }

        public bool Register(User user)
        {
            if (_users.Any(u => u.Username == user.Username || u.Email == user.Email))
                return false;

            _users.Add(user);
            return true;
        }
    }
} 