using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo {
    public interface IUserRepo {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
    }
}
