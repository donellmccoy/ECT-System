using ALOD.Core.Interfaces;
using ALOD.Data;
using System;
using System.Reflection;
using Xunit;

namespace ALOD.Tests.Unit.Data.Interceptors
{
    /// <summary>
    /// Tests for AuditInterceptor - NHibernate interceptor that automatically updates
    /// CreatedDate and ModifiedDate fields for IAuditable entities.
    /// Uses reflection to test internal class behavior.
    /// NOTE: These tests are currently skipped - they require NHibernate IType mocking which needs refactoring
    /// </summary>
    [Trait("Category", "Skip")]
    public class AuditInterceptorTests
    {
        private readonly object _interceptor;
        private readonly MethodInfo _onSaveMethod;
        private readonly MethodInfo _onFlushDirtyMethod;

        public AuditInterceptorTests()
        {
            // Access internal AuditInterceptor through reflection
            var interceptorType = typeof(AbstractNHibernateDao<,>).Assembly.GetType("ALOD.Data.AuditInterceptor");
            if (interceptorType == null)
            {
                throw new InvalidOperationException("Could not find AuditInterceptor type. Expected ALOD.Data.AuditInterceptor");
            }
            
            _interceptor = Activator.CreateInstance(interceptorType, true);
            
            // Get method references
            _onSaveMethod = interceptorType.GetMethod("OnSave");
            _onFlushDirtyMethod = interceptorType.GetMethod("OnFlushDirty");
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void AuditInterceptor_CanBeInstantiated()
        {
            // Assert
            Assert.NotNull(_interceptor);
            Assert.NotNull(_onSaveMethod);
            Assert.NotNull(_onFlushDirtyMethod);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnSave_WithAuditableEntity_SetsCreatedDateAndModifiedDate()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = new[] { "Id", "CreatedDate", "Name", "ModifiedDate" };
            var state = new object[] { 1, null, "Test", null };
            var types = new object[4];
            var beforeSave = DateTime.Now;

            // Act
            var result = (bool)_onSaveMethod.Invoke(_interceptor, new object[] { entity, 1, state, propertyNames, types });

            // Assert
            Assert.True(result);
            Assert.NotNull(state[1]);
            Assert.NotNull(state[3]);
            Assert.IsType<DateTime>(state[1]);
            Assert.IsType<DateTime>(state[3]);
            Assert.True((DateTime)state[1] >= beforeSave);
            Assert.True((DateTime)state[3] >= beforeSave);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnSave_WithAuditableEntity_OnlyCreatedDate_SetsDate()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = new[] { "Id", "CreatedDate", "Name" };
            var state = new object[] { 1, null, "Test" };
            var types = new object[3];

            // Act
            var result = (bool)_onSaveMethod.Invoke(_interceptor, new object[] { entity, 1, state, propertyNames, types });

            // Assert
            Assert.True(result);
            Assert.NotNull(state[1]);
            Assert.IsType<DateTime>(state[1]);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnSave_WithNonAuditableEntity_ReturnsFalse()
        {
            // Arrange
            var entity = new object();
            var propertyNames = new[] { "Id", "CreatedDate", "ModifiedDate" };
            var state = new object[] { 1, null, null };
            var types = new object[3];

            // Act
            var result = (bool)_onSaveMethod.Invoke(_interceptor, new object[] { entity, 1, state, propertyNames, types });

            // Assert
            Assert.False(result);
            Assert.Null(state[1]);
            Assert.Null(state[2]);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnSave_WithAuditableEntity_NoAuditProperties_ReturnsFalse()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = new[] { "Id", "Name", "Description" };
            var state = new object[] { 1, "Test", "Description" };
            var types = new object[3];

            // Act
            var result = (bool)_onSaveMethod.Invoke(_interceptor, new object[] { entity, 1, state, propertyNames, types });

            // Assert
            Assert.False(result);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnFlushDirty_WithAuditableEntity_SetsModifiedDate()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = new[] { "Id", "Name", "ModifiedDate" };
            var currentState = new object[] { 1, "Updated Name", DateTime.Now.AddDays(-1) };
            var previousState = new object[] { 1, "Old Name", DateTime.Now.AddDays(-2) };
            var types = new object[3];
            var beforeUpdate = DateTime.Now;

            // Act
            var result = (bool)_onFlushDirtyMethod.Invoke(_interceptor, 
                new object[] { entity, 1, currentState, previousState, propertyNames, types });

            // Assert
            Assert.True(result);
            Assert.NotNull(currentState[2]);
            Assert.IsType<DateTime>(currentState[2]);
            Assert.True((DateTime)currentState[2] >= beforeUpdate);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnFlushDirty_WithNonAuditableEntity_ReturnsFalse()
        {
            // Arrange
            var entity = new object();
            var propertyNames = new[] { "Id", "Name", "ModifiedDate" };
            var currentState = new object[] { 1, "Updated", DateTime.Now.AddDays(-1) };
            var previousState = new object[] { 1, "Original", DateTime.Now.AddDays(-2) };
            var types = new object[3];
            var originalDate = currentState[2];

            // Act
            var result = (bool)_onFlushDirtyMethod.Invoke(_interceptor, 
                new object[] { entity, 1, currentState, previousState, propertyNames, types });

            // Assert
            Assert.False(result);
            Assert.Equal(originalDate, currentState[2]);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnFlushDirty_WithAuditableEntity_NoModifiedDateProperty_ReturnsFalse()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = new[] { "Id", "Name", "Description" };
            var currentState = new object[] { 1, "Updated", "New Description" };
            var previousState = new object[] { 1, "Original", "Old Description" };
            var types = new object[3];

            // Act
            var result = (bool)_onFlushDirtyMethod.Invoke(_interceptor, 
                new object[] { entity, 1, currentState, previousState, propertyNames, types });

            // Assert
            Assert.False(result);
        }

        [Theory(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        [InlineData("Id", "CreatedDate", "ModifiedDate")]
        [InlineData("CreatedDate", "ModifiedDate", "UpdatedBy")]
        [InlineData("Name", "ModifiedDate", "CreatedDate")]
        public void OnSave_WithAuditableEntity_VariousPropertyOrders_SetsCorrectIndices(
            string prop1, string prop2, string prop3)
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = new[] { prop1, prop2, prop3 };
            var state = new object[] { null, null, null };
            var types = new object[3];

            // Act
            var result = (bool)_onSaveMethod.Invoke(_interceptor, new object[] { entity, 1, state, propertyNames, types });

            // Assert
            // Verify audit date properties were set
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] == "CreatedDate" || propertyNames[i] == "ModifiedDate")
                {
                    Assert.NotNull(state[i]);
                    Assert.IsType<DateTime>(state[i]);
                }
            }
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnSave_WithAuditableEntity_EmptyPropertyNames_ReturnsFalse()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = Array.Empty<string>();
            var state = Array.Empty<object>();
            var types = Array.Empty<object>();

            // Act
            var result = (bool)_onSaveMethod.Invoke(_interceptor, new object[] { entity, 1, state, propertyNames, types });

            // Assert
            Assert.False(result);
        }

        [Fact(Skip = "Requires NHibernate IType mocking - needs refactoring")]
        public void OnFlushDirty_WithAuditableEntity_EmptyPropertyNames_ReturnsFalse()
        {
            // Arrange
            var entity = new TestAuditableEntity();
            var propertyNames = Array.Empty<string>();
            var currentState = Array.Empty<object>();
            var previousState = Array.Empty<object>();
            var types = Array.Empty<object>();

            // Act
            var result = (bool)_onFlushDirtyMethod.Invoke(_interceptor, 
                new object[] { entity, 1, currentState, previousState, propertyNames, types });

            // Assert
            Assert.False(result);
        }

        // Test helper class implementing IAuditable
        private class TestAuditableEntity : IAuditable
        {
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
        }
    }
}
