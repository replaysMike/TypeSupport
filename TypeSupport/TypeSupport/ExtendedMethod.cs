using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeSupport
{
    /// <summary>
    /// Discovers the attributes of a method and provides access to method metadata
    /// </summary>
    public class ExtendedMethod
    {
        private readonly MethodInfo _methodInfo;
        private readonly Type _parentType;
        private readonly string[] _operatorOverloadNames = new string[]
        {
            "op_Implicit",
            "op_Explicit",
            "op_Addition",
            "op_Subtraction",
            "op_Multiply",
            "op_Division",
            "op_Modulus",
            "op_ExclusiveOr",
            "op_BitwiseAnd",
            "op_BitwiseOr",
            "op_LogicalAnd",
            "op_LogicalOr",
            "op_Assign",
            "op_LeftShift",
            "op_RightShift",
            "op_SignedRightShift",
            "op_UnsignedRightShift",
            "op_Equality",
            "op_GreaterThan",
            "op_LessThan",
            "op_Inequality",
            "op_GreaterThanOrEqual",
            "op_LessThanOrEqual",
            "op_MultiplicationAssignment",
            "op_SubtractionAssignment",
            "op_ExclusiveOrAssignment",
            "op_LeftShiftAssignment",
            "op_ModulusAssignment",
            "op_AdditionAssignment",
            "op_BitwiseAndAssignment",
            "op_BitwiseOrAssignment",
            "op_Comma",
            "op_DivisionAssignment",
            "op_Decrement",
            "op_Increment",
            "op_UnaryNegation",
            "op_UnaryPlus",
            "op_OnesComplement"
        };

        public MethodInfo MethodInfo
        {
            get {
                return _methodInfo;
            }
        }

        /// <summary>
        /// Gets the name of the current member
        /// </summary>
        public string Name => _methodInfo.Name;

        /// <summary>
        /// Gets the return type of this method
        /// </summary>
        public Type ReturnType => _methodInfo.ReturnType;

        /// <summary>
        /// Gets the type of this method
        /// </summary>
        public Type DeclaringType => _methodInfo.DeclaringType;

        /// <summary>
        /// True if the method is marked static
        /// </summary>
        public bool IsStatic => _methodInfo.IsStatic;

        /// <summary>
        /// True if the method is marked virtual
        /// </summary>
        public bool IsVirtual => _methodInfo.IsVirtual;

        /// <summary>
        /// True if the method is marked public
        /// </summary>
        public bool IsPublic => _methodInfo.IsPublic;

        /// <summary>
        /// True if the method is marked private
        /// </summary>
        public bool IsPrivate => _methodInfo.IsPrivate;

        /// <summary>
        /// True if the method is a generic method
        /// </summary>
        public bool IsGenericMethod => _methodInfo.IsGenericMethod;

        /// <summary>
        /// True if the method is marked abstract
        /// </summary>
        public bool IsAbstract => _methodInfo.IsAbstract;

        /// <summary>
        /// True if the method is marked final
        /// </summary>
        public bool IsFinal => _methodInfo.IsFinal;

        /// <summary>
        /// True if the method is an overriden method
        /// </summary>
        public bool IsOverride => _methodInfo.GetBaseDefinition().DeclaringType != _methodInfo.DeclaringType;

        /// <summary>
        /// True if the method is an overriden method
        /// </summary>
        public bool IsOverridden { get; }

        /// <summary>
        /// True if this method is a constructor
        /// </summary>
        public bool IsConstructor => _methodInfo.IsConstructor;

        /// <summary>
        /// True if this method signature is hidden by an overriding method
        /// </summary>
        public bool IsHideBySig => _methodInfo.IsHideBySig;

        /// <summary>
        /// True if this method is an auto property accessor
        /// </summary>
        public bool IsAutoPropertyAccessor { get; }

        /// <summary>
        /// True if this method is an auto property getter
        /// </summary>
        public bool IsGetter { get; }

        /// <summary>
        /// True if this method is an auto property setter
        /// </summary>
        public bool IsSetter { get; }

        /// <summary>
        /// True if this method is an operator overload
        /// </summary>
        public bool IsOperatorOverload { get; }

        /// <summary>
        /// Get the parameters for the method
        /// </summary>
        public IEnumerable<ParameterInfo> Parameters => _methodInfo.GetParameters();

        /// <summary>
        /// Create an extended property
        /// </summary>
        /// <param name="methodInfo"></param>
        public ExtendedMethod(MethodInfo methodInfo, Type parentType)
        {
            _methodInfo = methodInfo;
            _parentType = parentType;
            IsAutoPropertyAccessor = _methodInfo.IsSpecialName
                && _methodInfo.IsHideBySig
                && _methodInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any()
                && (_methodInfo.Name.Contains("get_") || _methodInfo.Name.Contains("set_"));
            IsGetter = IsAutoPropertyAccessor
                && (_methodInfo.Name.Contains("get_"));
            IsSetter = IsAutoPropertyAccessor
                && (_methodInfo.Name.Contains("set_"));
            IsOverridden = _parentType != null && _methodInfo.GetBaseDefinition().DeclaringType == _methodInfo.DeclaringType
            && _methodInfo.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static)
                .Any(x => x.Name == _methodInfo.Name && x.DeclaringType == _parentType);
            IsOperatorOverload = _methodInfo.IsSpecialName && _methodInfo.IsStatic && _operatorOverloadNames.Contains(_methodInfo.Name);
        }

        /// <summary>
        /// Create an extended property
        /// </summary>
        /// <param name="methodInfo"></param>
        public ExtendedMethod(MethodInfo methodInfo) : this(methodInfo, null)
        {
        }

        public static implicit operator ExtendedMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                return null;
            }
            return new ExtendedMethod(methodInfo);
        }

        public static implicit operator MethodInfo(ExtendedMethod extendedProperty)
        {
            if (extendedProperty == null)
                return null;
            return extendedProperty._methodInfo;
        }
        public override string ToString()
        {
            return $"{Name} {_methodInfo.ToString()}";
        }
    }
}
