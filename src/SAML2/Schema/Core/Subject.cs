using System;
using System.Xml.Serialization;
using SAML2.Schema.Protocol;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The optional &lt;Subject&gt; element specifies the principal that is the subject of all of the (zero or more)
    /// statements in the assertion.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class Subject
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Subject";

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("BaseID", typeof (BaseIDAbstract))]
        [XmlElement("EncryptedID", typeof (EncryptedElement))]
        [XmlElement("NameID", typeof (NameID))]
        [XmlElement("SubjectConfirmation", typeof (SubjectConfirmation))]
        public object[] Items { get; set; }

        #endregion
    }
}
