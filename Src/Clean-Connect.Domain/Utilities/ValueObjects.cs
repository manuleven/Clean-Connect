using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Utilities
{
    public abstract class ValueObjects
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            var other = (ValueObjects)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }


        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);    
        }

        public static bool operator ==(ValueObjects left, ValueObjects right)
        {
         
            return Equals(left, right);
        }

        public static  bool operator !=(ValueObjects left, ValueObjects right)
        {
            return !Equals(left, right);
        }
    }
}
