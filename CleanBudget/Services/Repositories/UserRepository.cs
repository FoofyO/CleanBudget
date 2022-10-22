using System;
using System.Text;
using System.Linq;
using System.Data.Entity;
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
                return db.Users.ToList();
            }
        }
        
        public User GetById(string id)
        {
            using (var db = new BudgetContext())
            {
                ICollection<User> users = GetAll();
                return users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            }
        }
        
        public void Update(User item)
        {
            using (var db = new BudgetContext())
            {
                ICollection<User> users = GetAll();
                var user = users.Where(u => u.Id.Equals(item.Id)).FirstOrDefault();

                if (user != null)
                {
                    user.Hash = item.Hash;
                    user.Salt = item.Salt;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChangesAsync();
                }
            }
        }
        
        public User Login(string id, string password)
        {
            using (var db = new BudgetContext())
            {
                ICollection<User> users = GetAll();
                var user = users.Where(u => u.Id.Equals(id)).FirstOrDefault();
                if (user != null)
                {
                    if (Decrypt(password, user.Salt).Equals(user.Hash))
                        return user;
                }
                return null;
            }
        }
        
        public string GetId(string email)
        {
            using (var db = new BudgetContext())
            {
                ICollection<User> users = GetAll();
                var result = users.Where(u => u.Email.Equals(email)).FirstOrDefault();
                if (result == null) return null;
                return result.Id;
            }
        }
        
        public static Tuple<string, string> Encrypt(string password)
        {
            var sha256 = SHA256.Create();
            var salt = Guid.NewGuid().ToString();
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] code = sha256.ComputeHash(unicodeEncoding.GetBytes(password + salt));
            return new Tuple<string, string>(Convert.ToBase64String(code), salt);
        }

        public static string Decrypt(string password, string salt)
        {
            var sha256 = SHA256.Create();
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] code = sha256.ComputeHash(unicodeEncoding.GetBytes(password + salt));
            return Convert.ToBase64String(code);
        }
    }
}
