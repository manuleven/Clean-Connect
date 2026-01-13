using Clean_Connect.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Value_Objects
{
    public class PhoneNumber : ValueObjects
    {
        public string Value { get;}
        private PhoneNumber() { }
        private PhoneNumber(string contact) 
        {
            Value = contact.Trim();
        }

        public static PhoneNumber Create(string contact)
        {
            if (string.IsNullOrWhiteSpace(contact))
                throw new ArgumentException("Invalid phone Number", nameof(contact));

            if (contact.Length < 11 || contact.Length > 15)
                throw new ArgumentOutOfRangeException("Phone number can't be less than 11 or greater than 15", nameof(contact));
           
            return new PhoneNumber(contact);

        }

        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phoneNumber) =>phoneNumber?.Value;
    }
}
