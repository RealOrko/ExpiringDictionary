using NUnit.Framework;
using System;
using System.Threading;
using ExpiringDictionaryWrapper;
using ExpiringDictionaryWrapper.Tests.Models;

namespace ExpiringDictionaryWrapper.Tests;

[TestFixture]
public class ExpiringDictionaryTests
{
    private ExpiringDictionaryWrapper<string, Person> _dict;

    [SetUp]
    public void SetUp()
    {
        _dict = new ExpiringDictionaryWrapper<string, Person>(
            valueToString: p => p.ToString(),
            stringToValue: Person.FromString
        );
    }

    [TearDown]
    public void TearDown()
    {
        _dict.Dispose();
    }

    [Test]
    public void InsertAndTryGet_ValidKey_ReturnsValue()
    {
        // Arrange
        var person = new Person { Name = "Alice", Age = 30 };
        _dict.Insert("key1", person, TimeSpan.FromSeconds(10));

        // Act
        bool result = _dict.TryGet("key1", out Person value);

        // Assert
        Assert.IsTrue(result, "Key should be found");
        Assert.AreEqual(person, value, "Retrieved value should match inserted value");
        Assert.AreEqual("Alice", value.Name, "Name should match");
        Assert.AreEqual(30, value.Age, "Age should match");
    }

    [Test]
    public void TryGet_ExpiredKey_ReturnsFalse()
    {
        // Arrange
        var person = new Person { Name = "Bob", Age = 25 };
        _dict.Insert("key2", person, TimeSpan.FromMilliseconds(100));

        // Act
        Thread.Sleep(150);
        bool result = _dict.TryGet("key2", out Person value);

        // Assert
        Assert.IsFalse(result, "Expired key should not be found");
        Assert.IsNull(value, "Value should be null for expired key");
    }

    [Test]
    public void Remove_ValidKey_KeyNoLongerExists()
    {
        // Arrange
        var person = new Person { Name = "Charlie", Age = 40 };
        _dict.Insert("key3", person, TimeSpan.FromSeconds(10));

        // Act
        _dict.Remove("key3");
        bool result = _dict.TryGet("key3", out Person value);

        // Assert
        Assert.IsFalse(result, "Removed key should not be found");
        Assert.IsNull(value, "Value should be null for removed key");
    }

    [Test]
    public void ContainsKey_ValidKey_ReturnsTrue()
    {
        // Arrange
        var person = new Person { Name = "David", Age = 35 };
        _dict.Insert("key4", person, TimeSpan.FromSeconds(10));

        // Act
        bool result = _dict.ContainsKey("key4");

        // Assert
        Assert.IsTrue(result, "ContainsKey should return true for existing key");
    }

    [Test]
    public void ContainsKey_ExpiredKey_ReturnsFalse()
    {
        // Arrange
        var person = new Person { Name = "Eve", Age = 28 };
        _dict.Insert("key5", person, TimeSpan.FromMilliseconds(100));

        // Act
        Thread.Sleep(150);
        bool result = _dict.ContainsKey("key5");

        // Assert
        Assert.IsFalse(result, "ContainsKey should return false for expired key");
    }

    [Test]
    public void Dispose_DictionaryDisposed_TryGetFails()
    {
        // Arrange
        var person = new Person { Name = "Frank", Age = 45 };
        _dict.Insert("key6", person, TimeSpan.FromSeconds(10));
        _dict.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => _dict.TryGet("key6", out _),
            "TryGet should throw after Dispose");
    }
}