﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Enumerable ConfigurationElementCollection abstract base class.
    /// </summary>
    /// <typeparam name="TConfigurationElementType">The type of the configuration element type.</typeparam>
    public abstract class EnumerableConfigurationElementCollection<TConfigurationElementType> :
        ConfigurationElementCollection, ICollection<TConfigurationElementType>
        where TConfigurationElementType : ConfigurationElement, IConfigurationElementCollectionElement, new()
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElementCollection"/> object is read only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Configuration.ConfigurationElementCollection"/> object is read only; otherwise, false.</returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>
        /// The specified property, attribute, or child element
        /// </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException" />
        public TConfigurationElementType this[int index]
        {
            get { return (TConfigurationElementType)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableConfigurationElementCollection&lt;TConfigurationElementType&gt;"/> class.
        /// </summary>
        public EnumerableConfigurationElementCollection()
        {}

        #region Implementation of IEnumerable<TConfigurationElementType>

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public new IEnumerator<TConfigurationElementType> GetEnumerator()
        {
            foreach (var type in (IEnumerable)this)
            {
                yield return (TConfigurationElementType)type;
            }
        }

        #endregion

        #region Overrides of ConfigurationElementCollection

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TConfigurationElementType();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ((TConfigurationElementType)element).ElementKey;
        }

        #endregion

        #region Implementation of ICollection<TConfigurationElementType>

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public void AddRange(IEnumerable<TConfigurationElementType> elements)
        {
            foreach (var element in elements)
            {
                BaseAdd(element);
            }
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(TConfigurationElementType item)
        {
            BaseAdd(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(TConfigurationElementType item)
        {
            return !(BaseIndexOf(item) < 0);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
        ///                     -or-
        ///                 <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        ///                     -or-
        ///                     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        ///                     -or-
        ///                     Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(TConfigurationElementType[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public bool Remove(TConfigurationElementType item)
        {
            BaseRemove(item);

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<TConfigurationElementType>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

