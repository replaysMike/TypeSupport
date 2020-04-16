namespace TypeSupport.Tests.TestObjects
{
    public class VisibilityObject
    {
        public bool PublicProperty { get; set; }
        private bool PrivateProperty { get; set; }
        protected bool ProtectedProperty { get; set; }
        internal bool InternalProperty { get; set; }

        public bool _publicField;
        private bool _privateField;
        protected bool _protectedField;
        internal bool _internalField;
    }
}
