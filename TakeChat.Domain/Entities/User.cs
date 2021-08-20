using System;

namespace TakeChat.Domain.Entities
{
    public class User
    {
        public string Name { get; }

        public User(string name)
        {
            if (string.IsNullOrEmpty(name.Trim()))
            {
                throw new ArgumentException("Nome de usuário não pode ser deixado em branco");
            }

            if (name.Trim().Length > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Nome de usuário não pode conter mais que 50 caracteres");
            }

            Name = name;
        }
    }
}
