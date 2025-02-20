using NUnit.Framework;
using System;
using System.Threading;
using ExpiringDictionaryWrapper;

namespace ExpiringDictionaryWrapper.Tests;

[TestFixture]
public class ExpiringDictionaryWrapperTests
{
    private ExpiringDictionaryWrapper _dict;

    [SetUp]
    public void SetUp()
    {
        _dict = new ExpiringDictionaryWrapper();
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
        string key = "key1";
        string value = "value1";
        _dict.Insert(key, value, TimeSpan.FromSeconds(10));

        // Act
        bool result = _dict.TryGet(key, out string retrievedValue);

        // Assert
        Assert.IsTrue(result, "Key should be found");
        Assert.AreEqual(value, retrievedValue, "Retrieved value should match inserted value");
    }

    [Test]
    public void TryGet_ExpiredKey_ReturnsFalse()
    {
        // Arrange
        string key = "key2";
        string value = "value2";
        _dict.Insert(key, value, TimeSpan.FromMilliseconds(100));

        // Act
        Thread.Sleep(150); // Wait for expiry
        bool result = _dict.TryGet(key, out string retrievedValue);

        // Assert
        Assert.IsFalse(result, "Expired key should not be found");
        Assert.IsNull(retrievedValue, "Value should be null for expired key");
    }

    [Test]
    public void Remove_ValidKey_KeyNoLongerExists()
    {
        // Arrange
        string key = "key3";
        string value = "value3";
        _dict.Insert(key, value, TimeSpan.FromSeconds(10));

        // Act
        _dict.Remove(key);
        bool result = _dict.TryGet(key, out string retrievedValue);

        // Assert
        Assert.IsFalse(result, "Removed key should not be found");
        Assert.IsNull(retrievedValue, "Value should be null for removed key");
    }

    [Test]
    public void ContainsKey_ValidKey_ReturnsTrue()
    {
        // Arrange
        string key = "key4";
        string value = "value4";
        _dict.Insert(key, value, TimeSpan.FromSeconds(10));

        // Act
        bool result = _dict.ContainsKey(key);

        // Assert
        Assert.IsTrue(result, "ContainsKey should return true for existing key");
    }

    [Test]
    public void ContainsKey_ExpiredKey_ReturnsFalse()
    {
        // Arrange
        string key = "key5";
        string value = "value5";
        _dict.Insert(key, value, TimeSpan.FromMilliseconds(100));

        // Act
        Thread.Sleep(150); // Wait for expiry
        bool result = _dict.ContainsKey(key);

        // Assert
        Assert.IsFalse(result, "ContainsKey should return false for expired key");
    }

    [Test]
    public void Dispose_DictionaryDisposed_TryGetFails()
    {
        // Arrange
        string key = "key6";
        string value = "value6";
        _dict.Insert(key, value, TimeSpan.FromSeconds(10));
        _dict.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => _dict.TryGet(key, out _),
            "TryGet should throw after Dispose");
    }
}