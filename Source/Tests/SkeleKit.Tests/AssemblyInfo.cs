using Xunit;

// the theme is app-global static state; a parallel test class would see another's styles
[assembly: CollectionBehavior(DisableTestParallelization = true)]
