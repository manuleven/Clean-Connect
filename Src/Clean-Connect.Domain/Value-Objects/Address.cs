using Clean_Connect.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Value_Objects
{
    public class Address : ValueObjects
    {

        public string Value { get; }

        private Address() { }

        private Address(string value)
        {
            Value = value.Trim();
        }

        public static Address Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Address cannot be null or empty", nameof(value));
            if (value.Length > 200)
                throw new ArgumentException("Address cannot be longer than 200 characters", nameof(value));

            return new Address(value);
        }
      
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value; 
        }

        public override string ToString() 
        { 
            return Value;
        }

        public static implicit operator string(Address address) => address?.Value;
    }   
}
