using System.Collections.Generic;

namespace GameplayAbilities.Runtime.Attributes {
    /// <summary>
    /// An interface for anything that can read attribute values from an owning game object.
    /// </summary>
    public interface IAttributeReader : IEnumerable<Attribute> {
        /// <summary>
        /// Get the current value of an attribute.
        /// </summary>
        /// <param name="key">The key of the attribute.</param>
        /// <returns>The current value of the attribute.</returns>
        public int GetCurrent(string key);
        
        /// <summary>
        /// Get the maximum value of an attribute.
        /// </summary>
        /// <param name="key">The key of the attribute.</param>
        /// <returns>The maximum value of an attribute, or <c>int.MaxValue</c> for unbounded attributes.</returns>
        public int GetMax(string key);
        
        /// <summary>
        /// Get the minimum value of an attribute.
        /// </summary>
        /// <param name="key">The key of the attribute</param>
        /// <returns>The minimum value of the attribute, or <c>int.MinValue</c> for unbounded attributes.</returns>
        public int GetMin(string key);
        
        /// <summary>
        /// Get the numerical data of an attribute.
        /// </summary>
        /// <param name="key">The key of the attribute.</param>
        /// <returns>An <see cref="Attribute"/> object containing the numerical data of the attribute.</returns>
        public Attribute GetAttribute(string key);

        /// <summary>
        /// Checks if the owner has sufficient amount of attribute for a given key.
        /// </summary>
        /// <param name="threshold">The threshold to check against.</param>
        /// <param name="key">The key of the attribute.</param>
        /// <returns><c>true</c> if the owner has sufficient amount of attribute for the given key.</returns>
        public bool Has(int threshold, string key);
    }
}
