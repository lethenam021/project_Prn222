using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class UserRepository : IUserRepo {
        private readonly CloneEbayDbContext _context;

        public UserRepository(CloneEbayDbContext context) {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User> GetUserByIdAsync(int userId) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user!;
        }

        public async Task AddUserAsync(User user) {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
 