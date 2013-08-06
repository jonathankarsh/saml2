﻿using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// NameIdFormat configuration element.
    /// </summary>
    public class NameIdFormatElement : ConfigurationElement
    {
        /// <summary>
        /// The RegEx string used to validate the Format attribute.
        /// </summary>
        private const string NameIdFormatsRegex = @"^(urn:oasis:names:tc:SAML:2\.0:nameid-format:persistent|urn:oasis:names:tc:SAML:2\.0:nameid-format:transient|urn:oasis:names:tc:SAML:1\.1:nameid-format:emailAddress|urn:oasis:names:tc:SAML:1\.1:nameid-format:unspecified|urn:oasis:names:tc:SAML:1\.1:nameid-format:X509SubjectName|urn:oasis:names:tc:SAML:1\.1:nameid-format:WindowsDomainQualifiedName|urn:oasis:names:tc:SAML:2\.0:nameid-format:kerberos|urn:oasis:names:tc:SAML:2\.0:nameid-format:entity)$";

        /// <summary>
        /// Gets the type.
        /// </summary>
        [ConfigurationProperty("format", IsKey = true, IsRequired = true)]
        [RegexStringValidator(NameIdFormatsRegex)]
        public string Format
        {
            get { return (string)base["format"]; }
        }
    }
}
