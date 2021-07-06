using SatoCore.Attributes;
using SatoCore.Extensions;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SatoCore
{
    /// <summary>Represents a data model store.</summary>
    /// <typeparam name="T">The type of data models to store.</typeparam>
    /// <typeparam name="TIdentifier">The type of the property with the <see cref="IdentifierAttribute"/> in the model.</typeparam>
    public class Repository<T, TIdentifier>
        where T : class
    {
        /*********
        ** Fields
        *********/
        /// <summary>The monitor to use for logging.</summary>
        private readonly IMonitor Monitor;

        /// <summary>The items stored in the repository.</summary>
        private readonly List<T> Items = new List<T>();


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="monitor">The monitor to use for logging.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="monitor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/>'s identifier isn't valid or if <typeparamref name="T"/> has an invalid editable property.</exception>
        public Repository(IMonitor monitor)
        {
            Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // ensure the identifier is valid
            {
                // ensure T has a single identifier
                var identifierProperties = properties.Where(member => member.HasCustomAttribute<IdentifierAttribute>());
                if (identifierProperties.Count() != 1)
                    throw new ArgumentException($"Type: '{typeof(T).FullName}' doesn't have one member with an '{nameof(IdentifierAttribute)}', instead it has {identifierProperties.Count()}");
                var identifierProperty = identifierProperties.First();

                // ensure identifier isn't editable
                if (identifierProperty.HasCustomAttribute<EditableAttribute>())
                    throw new ArgumentException($"'{identifierProperty.GetFullName()}' is marked as the identifier by also editable");

                // ensure identifier is readable
                if (!identifierProperty.CanRead)
                    throw new ArgumentException($"'{identifierProperty.GetFullName()}' is marked as the identifier by not readable");

                // ensure identifier is type TIdentifier
                if (identifierProperty.PropertyType != typeof(TIdentifier))
                    throw new ArgumentException($"{nameof(TIdentifier)} ({typeof(TIdentifier).FullName}) doesn't match the identifier member ({identifierProperties.ElementAt(0).GetType().FullName})");
            }

            // ensure all editable/defaultable properties are valid
            {
                var editableProperties = properties.Where(property => property.HasCustomAttribute<EditableAttribute>() || property.HasCustomAttribute<DefaultValueAttribute>());
                foreach (var editableProperty in editableProperties)
                {
                    // ensure property is nullable
                    var type = editableProperty.PropertyType;
                    if (type.IsValueType && (Nullable.GetUnderlyingType(type) == null))
                        throw new ArgumentException($"'{editableProperty.GetFullName()}' is marked as editable/defaultable but not nullable");

                    // ensure property is readable & writable
                    if (!editableProperty.CanRead || !editableProperty.CanWrite)
                        throw new ArgumentException($"'{editableProperty.GetFullName()}' is marked as editable/defaultable but not readable and writable");
                }
            }
        }

        /// <summary>Adds an item to the repository.</summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null"/>.</exception>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // ensure all required members are present
            if (!ValidateRequiredProperties(item))
                return;

            // ensure item doesn't already exist in the repository
            var identifier = GetIdentifier(item);
            if (identifier == null)
            {
                Monitor.Log($"Tried to add an item ({typeof(T).Name}) without specifying the identifier", LogLevel.Error);
                return;
            }

            if (Get(identifier) != null)
            {
                Monitor.Log($"An item ({typeof(T).Name}) with the id: '{identifier}' already exists (trying to add)", LogLevel.Error);
                return;
            }

            // set all null values to default values
            var defaultableProperties = GetDefaultableProperties();
            foreach (var property in defaultableProperties)
                if (property.GetValue(item) == null)
                    property.SetValue(item, property.GetCustomAttribute<DefaultValueAttribute>().DefaultValue);

            // add item
            Items.Add(item);
        }

        /// <summary>Edits an item.</summary>
        /// <param name="item">The item containing the identifier of the item to edit, and the new values of the item.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null"/>.</exception>
        public void Edit(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // ensure the item exists in the repository
            var identifier = GetIdentifier(item);
            if (identifier == null)
            {
                Monitor.Log($"Tried to edit an item ({typeof(T).Name}) without specifying the identifier", LogLevel.Error);
                return;
            }

            var itemToEdit = Get(identifier);
            if (itemToEdit == null)
            {
                Monitor.Log($"An item ({typeof(T).Name}) with the id: '{identifier}' doesn't exists (trying to edit)", LogLevel.Error);
                return;
            }

            // edit properties
            var editableProperties = GetEditableProperties();
            foreach (var property in editableProperties)
                property.SetValue(itemToEdit, property.GetValue(item) ?? property.GetValue(itemToEdit));
        }

        /// <summary>Deletes an item.</summary>
        /// <param name="id">The id of the item to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null"/>.</exception>
        public void Delete(TIdentifier id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            // ensure the item exists in the repository
            var itemToDelete = Get(id);
            if (itemToDelete == null)
            {
                Monitor.Log($"An item ({typeof(T).Name}) with the id: '{id}' doesn't exists (trying to edit)", LogLevel.Error);
                return;
            }

            // delete item
            Items.Remove(itemToDelete);
        }

        /// <summary>Retrieves an item by id.</summary>
        /// <param name="id">The id of the item to retrieve.</param>
        /// <returns>An item with the id of <paramref name="id"/>, if it exists; otherwise, <see langword="null"/>.</returns>
        public T Get(TIdentifier id)
        {
            if (typeof(TIdentifier) == typeof(string))
                return Items.FirstOrDefault(item => GetIdentifier(item).ToString().ToLower() == id.ToString().ToLower());
            else
                return Items.FirstOrDefault(item => GetIdentifier(item).Equals(id));
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Retrieves the identifier of an item.</summary>
        /// <param name="item">The item whose identifier should be retrieved.</param>
        /// <returns>The identitifer of <paramref name="item"/>.</returns>
        private TIdentifier GetIdentifier(T item) => (TIdentifier)GetIdentifierProperties().FirstOrDefault()?.GetValue(item);

        /// <summary>Retrieves all the properties that have an <see cref="IdentifierAttribute"/> in <typeparamref name="T"/>.</summary>
        /// <returns>The properties that have an <see cref="IdentifierAttribute"/>.</returns>
        private IEnumerable<PropertyInfo> GetIdentifierProperties() => GetPropertiesWithAttribute<IdentifierAttribute>();

        /// <summary>Retrieves all properties that have an <see cref="EditableAttribute"/> in <typeparamref name="T"/>.</summary>
        /// <returns>The properties that have an <see cref="EditableAttribute"/>.</returns>
        private IEnumerable<PropertyInfo> GetEditableProperties() => GetPropertiesWithAttribute<EditableAttribute>();

        /// <summary>Retrieves all properties that have a <see cref="DefaultValueAttribute"/> in <typeparamref name="T"/>.</summary>
        /// <returns>The properties that have a <see cref="DefaultValueAttribute"/>.</returns>
        private IEnumerable<PropertyInfo> GetDefaultableProperties() => GetPropertiesWithAttribute<DefaultValueAttribute>();

        /// <summary>Retrieves all properties with a specified attribute.</summary>
        /// <typeparam name="T">The type of attribute to get the properties which have it.</typeparam>
        /// <returns>The properties which have an attribute of type <typeparamref name="T"/>.</returns>
        private IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return typeof(T)
                .GetInstanceProperties()
                .Where(property => property.HasCustomAttribute<TAttribute>());
        }

        /// <summary>Checks if all required properties are valid.</summary>
        /// <param name="item">The item whose properties should be checked.</param>
        /// <returns><see langword="true"/>, if all the required properties are valid; otherwise, <see langword="false"/>.</returns>
        private bool ValidateRequiredProperties(T item)
        {
            // retrieve required properties
            var properties = typeof(T)
                .GetInstanceProperties()
                .Where(property => property.HasCustomAttribute<RequiredAttribute>());

            // check required properties
            var isValid = true;
            foreach (var property in properties)
                if ((property.PropertyType == typeof(string) && string.IsNullOrWhiteSpace((string)property.GetValue(item)))
                    || property.GetValue(item) == null)
                {
                    Monitor.Log($"Tried to add an item ({typeof(T).Name}) without specifying '{property.Name}'", LogLevel.Error);
                    isValid = false;
                }

            return isValid;
        }
    }
}
