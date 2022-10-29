using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

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
                return GetAll().FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public void Update(User item)
        {
            using (var db = new BudgetContext())
            {
                var user = GetAll().FirstOrDefault(u => u.Id.Equals(item.Id));

                if (user != null)
                {
                    user.Email = item.Email;
                    user.Firstname = item.Firstname;
                    user.Lastname = item.Lastname;
                    user.Hash = item.Hash;
                    user.Salt = item.Salt;
                    db.Users.Update(user);
                    db.SaveChanges();
                }
            }
        }

        public Guid GetId(string email)
        {
            using (var db = new BudgetContext())
            {
                var result = GetAll().FirstOrDefault(u => u.Email.Equals(email));
                if (result == null) return Guid.Empty;
                return result.Id;
            }
        }

        public User Login(Guid id, string password)
        {
            using (var db = new BudgetContext())
            {
                var user = GetAll().FirstOrDefault(u => u.Id.Equals(id));
                if (user != null)
                {
                    if (Decrypt(password, user.Salt).Equals(user.Hash))
                        return user;
                }
                return null;
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
