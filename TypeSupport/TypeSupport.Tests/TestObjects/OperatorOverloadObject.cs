namespace TypeSupport.Tests.TestObjects
{
    public class OperatorOverloadObject
    {
        public int Id { get; set; }

        public override int GetHashCode() => Id;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(OperatorOverloadObject))
                return false;
            var typedObj = (OperatorOverloadObject)obj;
            return Id.Equals(typedObj.Id);
        }

        public static bool operator ==(OperatorOverloadObject a, OperatorOverloadObject b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;
            return a.Id.Equals(b.Id);
        }

        public static bool operator !=(OperatorOverloadObject a, OperatorOverloadObject b) => !a.Equals(b);
    }
}
