// net472 lacks the compiler-recognized attributes for `init`, `required`, and `[SetsRequiredMembers]`.
// These marker types let the ported hot-reload code compile unchanged. All internal, so they never
// clash with another assembly's copies.

namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
	internal sealed class RequiredMemberAttribute : Attribute;

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	internal sealed class CompilerFeatureRequiredAttribute(
		string featureName) : Attribute
	{
		public string FeatureName { get; } = featureName;
	}
}

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
	internal sealed class SetsRequiredMembersAttribute : Attribute;
}
