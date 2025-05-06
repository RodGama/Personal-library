using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Users.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        private string _email;
        private string _name;
        private string _password;
        private DateOnly _birthDate;

        public string Email
        {
            get => _email;
            set => _email = value?.Trim();
        }

        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        public string Password
        {
            get => _password;
            set => _password = value?.Trim(); 
        }

        public DateOnly BirthDate
        {
            get => _birthDate;
            set => _birthDate = value;
        }
    }
}
