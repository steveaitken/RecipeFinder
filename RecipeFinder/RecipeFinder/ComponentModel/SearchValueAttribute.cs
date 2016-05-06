using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeFinder.ComponentModel
{
    /// <devdoc>
    ///    <para>Specifies a description for a property
    ///       or event.</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.All)]
    public class SearchValueAttribute : Attribute
    {
        /// <devdoc>
        /// <para>Specifies the default value for the <see cref='RecipeFinder.ComponentModel.SearchValueAttribute'/> , which is an
        ///    empty string (""). This <see langword='static'/> field is read-only.</para>
        /// </devdoc>
        public static readonly SearchValueAttribute Default = new SearchValueAttribute();
        private string searchValue;

        public SearchValueAttribute() : this (string.Empty) {
        }

        public SearchValueAttribute(string description)
        {
            this.searchValue = description;
        }

        public virtual string SearchValue
        {
            get
            {
                return SearchValueText;
            }
        }

        protected string SearchValueText
        {
            get
            {
                return searchValue;
            }
            set
            {
                searchValue = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            SearchValueAttribute other = obj as SearchValueAttribute;

            return (other != null) && other.SearchValue == SearchValue;
        }

        public override int GetHashCode()
        {
            return SearchValue.GetHashCode();
        }
    }
}