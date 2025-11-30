using ALOD.Core.Domain.Common;
using ALOD.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Xunit;

namespace ALOD.Tests.Unit.Data.Types
{
    /// <summary>
    /// Tests for XmlType&lt;T&gt; and derived types - NHibernate custom user types for XML serialization.
    /// Tests basic type properties and serialization/deserialization logic.
    /// </summary>
    public class XmlTypeTests
    {
        [Fact]
        public void PersonnelDataType_IsMutable_ReturnsTrue()
        {
            // Arrange
            var type = CreatePersonnelDataType();

            // Act
            var isMutable = (bool)GetProperty(type, "IsMutable");

            // Assert
            Assert.True(isMutable);
        }

        [Fact]
        public void PersonnelDataType_ReturnedType_ReturnsPersonnelData()
        {
            // Arrange
            var type = CreatePersonnelDataType();

            // Act
            var returnedType = (Type)GetProperty(type, "ReturnedType");

            // Assert
            Assert.Equal(typeof(PersonnelData), returnedType);
        }

        [Fact]
        public void PersonnelDataListType_ReturnedType_ReturnsListOfPersonnelData()
        {
            // Arrange
            var type = CreatePersonnelDataListType();

            // Act
            var returnedType = (Type)GetProperty(type, "ReturnedType");

            // Assert
            Assert.Equal(typeof(List<PersonnelData>), returnedType);
        }

        [Fact]
        public void WitnessDataType_IsMutable_ReturnsTrue()
        {
            // Arrange
            var type = new WitnessDataType();

            // Act
            var isMutable = (bool)GetProperty(type, "IsMutable");

            // Assert
            Assert.True(isMutable);
        }

        [Fact]
        public void WitnessDataType_ReturnedType_ReturnsWitnessData()
        {
            // Arrange
            var type = new WitnessDataType();

            // Act
            var returnedType = (Type)GetProperty(type, "ReturnedType");

            // Assert
            Assert.Equal(typeof(WitnessData), returnedType);
        }

        [Fact]
        public void WitnessDataListType_ReturnedType_ReturnsListOfWitnessData()
        {
            // Arrange
            var type = new WitnessDataListType();

            // Act
            var returnedType = (Type)GetProperty(type, "ReturnedType");

            // Assert
            Assert.Equal(typeof(List<WitnessData>), returnedType);
        }

        [Fact]
        public void SqlXmlType_CanBeInstantiated()
        {
            // Arrange & Act
            var sqlXmlType = new SqlXmlType();

            // Assert
            Assert.NotNull(sqlXmlType);
        }

        [Fact]
        public void SqlTypes_ReturnsArrayWithSqlXmlType()
        {
            // Arrange
            var type = new WitnessDataType();

            // Act
            var sqlTypes = (Array)GetProperty(type, "SqlTypes");

            // Assert
            Assert.NotNull(sqlTypes);
            Assert.Single(sqlTypes);
            Assert.IsType<SqlXmlType>(sqlTypes.GetValue(0));
        }

        [Fact]
        public void Assemble_ReturnsInput()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var cached = new PersonnelData { SSN = "123456789" };

            // Act
            var method = type.GetType().GetMethod("Assemble");
            var result = method.Invoke(type, new object[] { cached, null });

            // Assert
            Assert.Same(cached, result);
        }

        [Fact]
        public void Disassemble_ReturnsInput()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var value = new PersonnelData { SSN = "123456789" };

            // Act
            var method = type.GetType().GetMethod("Disassemble");
            var result = method.Invoke(type, new object[] { value });

            // Assert
            Assert.Same(value, result);
        }

        [Fact]
        public void Replace_ReturnsOriginal()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var original = new PersonnelData { SSN = "123456789" };
            var target = new PersonnelData { SSN = "987654321" };

            // Act
            var method = type.GetType().GetMethod("Replace");
            var result = method.Invoke(type, new object[] { original, target, null });

            // Assert
            Assert.Same(original, result);
        }

        [Fact]
        public void DeepCopy_CreatesNewInstance()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var original = new PersonnelData 
            { 
                SSN = "123456789",
                Name = "John Doe",
                Grade = "E5"
            };

            // Act
            var method = type.GetType().GetMethod("DeepCopy");
            var copy = (PersonnelData)method.Invoke(type, new object[] { original });

            // Assert
            Assert.NotSame(original, copy);
            Assert.Equal(original.SSN, copy.SSN);
            Assert.Equal(original.Name, copy.Name);
            Assert.Equal(original.Grade, copy.Grade);
        }

        [Fact]
        public void DeepCopy_WithList_CreatesNewList()
        {
            // Arrange
            var type = CreatePersonnelDataListType();
            var original = new List<PersonnelData>
            {
                new PersonnelData { SSN = "111111111", Name = "John" },
                new PersonnelData { SSN = "222222222", Name = "Jane" }
            };

            // Act
            var method = type.GetType().GetMethod("DeepCopy");
            var copy = (List<PersonnelData>)method.Invoke(type, new object[] { original });

            // Assert
            Assert.NotSame(original, copy);
            Assert.Equal(original.Count, copy.Count);
            Assert.Equal(original[0].SSN, copy[0].SSN);
            Assert.Equal(original[1].Name, copy[1].Name);
        }

        [Fact]
        public void Equals_WithIdenticalObjects_ReturnsTrue()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var obj1 = new PersonnelData { SSN = "123456789", Name = "John" };
            var obj2 = new PersonnelData { SSN = "123456789", Name = "John" };

            // Act
            var method = type.GetType().GetMethod("Equals", new[] { typeof(object), typeof(object) });
            var result = (bool)method.Invoke(type, new object[] { obj1, obj2 });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentObjects_ReturnsFalse()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var obj1 = new PersonnelData { SSN = "123456789", Name = "John" };
            var obj2 = new PersonnelData { SSN = "987654321", Name = "Jane" };

            // Act
            var method = type.GetType().GetMethod("Equals", new[] { typeof(object), typeof(object) });
            var result = (bool)method.Invoke(type, new object[] { obj1, obj2 });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ReturnsObjectHashCode()
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var obj = new PersonnelData { SSN = "123456789" };

            // Act
            var method = type.GetType().GetMethod("GetHashCode", new[] { typeof(object) });
            var hashCode = (int)method.Invoke(type, new object[] { obj });

            // Assert
            Assert.Equal(obj.GetHashCode(), hashCode);
        }

        [Theory]
        [InlineData("123456789", "John Doe", "E5")]
        [InlineData("987654321", "Jane Smith", "O3")]
        [InlineData("555555555", "", "")]
        public void DeepCopy_PreservesAllProperties(string ssn, string name, string grade)
        {
            // Arrange
            var type = CreatePersonnelDataType();
            var original = new PersonnelData 
            { 
                SSN = ssn,
                Name = name,
                Grade = grade
            };

            // Act
            var method = type.GetType().GetMethod("DeepCopy");
            var copy = (PersonnelData)method.Invoke(type, new object[] { original });

            // Assert
            Assert.Equal(original.SSN, copy.SSN);
            Assert.Equal(original.Name, copy.Name);
            Assert.Equal(original.Grade, copy.Grade);
        }

        [Fact]
        public void WitnessDataType_DeepCopy_PreservesWitnessData()
        {
            // Arrange
            var type = new WitnessDataType();
            var original = new WitnessData 
            { 
                Name = "John Witness",
                Address = "123 Main St"
            };

            // Act
            var method = type.GetType().GetMethod("DeepCopy");
            var copy = (WitnessData)method.Invoke(type, new object[] { original });

            // Assert
            Assert.NotSame(original, copy);
            Assert.Equal(original.Name, copy.Name);
            Assert.Equal(original.Address, copy.Address);
        }

        [Fact]
        public void WitnessDataListType_DeepCopy_PreservesList()
        {
            // Arrange
            var type = new WitnessDataListType();
            var original = new List<WitnessData>
            {
                new WitnessData { Name = "Witness 1", Address = "Address 1" },
                new WitnessData { Name = "Witness 2", Address = "Address 2" },
                new WitnessData { Name = "Witness 3", Address = "Address 3" }
            };

            // Act
            var method = type.GetType().GetMethod("DeepCopy");
            var copy = (List<WitnessData>)method.Invoke(type, new object[] { original });

            // Assert
            Assert.NotSame(original, copy);
            Assert.Equal(original.Count, copy.Count);
            for (int i = 0; i < original.Count; i++)
            {
                Assert.Equal(original[i].Name, copy[i].Name);
                Assert.Equal(original[i].Address, copy[i].Address);
            }
        }

        // Helper methods to create instances of internal types
        private object CreatePersonnelDataType()
        {
            var type = typeof(XmlType<>).Assembly.GetType("ALOD.Data.Types.PersonnelDataType");
            return Activator.CreateInstance(type, true);
        }

        private object CreatePersonnelDataListType()
        {
            var type = typeof(XmlType<>).Assembly.GetType("ALOD.Data.Types.PersonnelDataListType");
            return Activator.CreateInstance(type, true);
        }

        private object GetProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property.GetValue(obj);
        }
    }
}
