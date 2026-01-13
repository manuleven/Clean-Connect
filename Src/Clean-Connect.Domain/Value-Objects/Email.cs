using Clean_Connect.Domain.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Value_Objects
{
    public class Email : ValueObjects
    {

        public string Value { get; }

        private Email() { }
        private Email(string value)
        { 
            Value = value.Trim();
        }

        public static Email Create(string email)
        {

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Invalid Email", nameof(email));

            if (email.Length > 100 || email.Length < 10)
                throw new ArgumentOutOfRangeException("Email must be greater than 10 or less than 100", nameof(email));

            email = email.Trim().ToLowerInvariant();
            return new Email(email);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email?.Value;
       
    }
}
