using KellermanSoftware.CompareNetObjects;
using Xunit.Sdk;

namespace PV260.Common.Tests;

/// <summary>
/// Provides methods for deep comparison assertions in unit tests.
/// </summary>
public static class DeepAssert
{
    /// <summary>
    /// Asserts that two objects are deeply equal, with optional properties to ignore during comparison.
    /// </summary>
    /// <typeparam name="T">The type of the objects being compared.</typeparam>
    /// <param name="expected">The expected object.</param>
    /// <param name="actual">The actual object.</param>
    /// <param name="propertiesToIgnore">An array of property names to ignore during comparison.</param>
    /// <exception cref="EqualException">Thrown when the objects are not deeply equal.</exception>
    public static void Equal<T>(T? expected, T? actual, params string[] propertiesToIgnore)
    {
        CompareLogic compareLogic = new()
        {
            Config =
            {
                MembersToIgnore = propertiesToIgnore.ToList(),
                IgnoreCollectionOrder = true,
                IgnoreObjectTypes = true,
                CompareStaticProperties = false,
                CompareStaticFields = false
            }
        };

        var comparisonResult = compareLogic.Compare(expected!, actual!);
        if (!comparisonResult.AreEqual)
        {
            throw EqualException.ForMismatchedValues(expected, actual, comparisonResult.DifferencesString);
        }
    }

    /// <summary>
    /// Asserts that a collection contains an object that is deeply equal to the expected object, 
    /// with optional properties to ignore during comparison.
    /// </summary>
    /// <typeparam name="T">The type of the objects being compared.</typeparam>
    /// <param name="expected">The expected object to find in the collection.</param>
    /// <param name="collection">The collection to search.</param>
    /// <param name="propertiesToIgnore">An array of property names to ignore during comparison.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    /// <exception cref="ContainsException">Thrown when the expected object is not found in the collection.</exception>
    public static void Contains<T>(T? expected, IEnumerable<T>? collection, params string[] propertiesToIgnore)
    {
        ArgumentNullException.ThrowIfNull(collection);

        CompareLogic compareLogic = new()
        {
            Config =
            {
                MembersToIgnore = propertiesToIgnore.ToList(),
                IgnoreCollectionOrder = true,
                IgnoreObjectTypes = true,
                CompareStaticProperties = false,
                CompareStaticFields = false
            }
        };

        if (!collection.Any(item => compareLogic.Compare(expected!, item).AreEqual))
        {
            throw ContainsException.ForCollectionItemNotFound(expected!.ToString()!, nameof(collection));
        }
    }
}