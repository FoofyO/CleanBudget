using System;
using System.Text;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CleanBudget.Services.Repositories
{
    public class UserRepository : IRepository<User>
    {
        public void Create(User item)
        {
            using (var db = new BudgetContext())
            {
                db.Users.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(User item)
        {
            using (var db = new BudgetContext())
            {
                db.Users.Remove(item);
                db.SaveChanges();
            }
        }

        public ICollection<User> GetAll()
        {
            using (var db = new BudgetContext())
            {
                List<User> users = null;// = db.Users.;
                return users;
            }
        }

        public User GetById(int id)
        {
            using (var db = new BudgetContext())
            {
                User user = null;// = db.Users.;
                return user;
            }
        }

        public void Update(User item)
        {
            using (var db = new BudgetContext())
            {
                var user = db.Users.SingleOrDefault(u => item.Id == u.Id);

                if (user != null)
                {
                    user.Hash = item.Hash;
                    user.Salt = item.Salt;
                    db.SaveChanges();
                }
            }
        }

        public User Login(string id, string password)
        {
            using (var db = new BudgetContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == id);
                if (user != null)
                {
                    var sha256 = SHA256.Create();
                    UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
                    byte[] code = sha256.ComputeHash(unicodeEncoding.GetBytes(password + user.Salt));
                    string cryptedPass = Convert.ToBase64String(code);
                    if (cryptedPass.Equals(user.Hash)) return user;
                }
                return null;
            }
        }

        public string GetIdByString(string username)
        {
            using (var db = new BudgetContext())
            {
                var result = db.Users.SingleOrDefault(u => u.Email == username);
                if (result == null) return string.Empty;
                else return result.Id;
            }
        }

        public static Tuple<string, string> Cript(string password)
        {
            var sha256 = SHA256.Create();
            var salt = Guid.NewGuid().ToString();
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] code = sha256.ComputeHash(unicodeEncoding.GetBytes(password + salt));
            string cryptedPass = Convert.ToBase64String(code);
            return new Tuple<string, string>(salt, cryptedPass);
        }
    }
}
