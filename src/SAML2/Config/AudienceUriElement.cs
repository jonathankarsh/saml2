﻿using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Audience configuration element.
    /// </summary>
    public class AudienceUriElement : ConfigurationElement, IConfigurationElementCollectionElement
    {
        /// <summary>
        /// Gets the URI.
        /// </summary>
        [ConfigurationProperty("uri", IsKey = true, IsRequired = true)]
        public string Uri
        {
            get { return (string) base["uri"]; }
        }

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Uri; }
        }

        #endregion
    }
}
