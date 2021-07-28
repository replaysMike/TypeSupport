namespace TypeSupport.Tests.TestObjects
{
    public class VisibilityObject
    {
        public bool PublicProperty { get; set; }
        private bool PrivateProperty { get; set; }
        protected bool ProtectedProperty { get; set; }
        internal bool InternalProperty { get; set; }

        public bool PublicField = false;
        private bool _privateField = false;
        protected bool ProtectedField = false;
        internal bool _internalField = false;
    }
}
