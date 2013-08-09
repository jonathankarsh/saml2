using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The &lt;StatusDetail&gt; element MAY be used to specify additional information concerning the status of
    /// the request. The additional information consists of zero or more elements from any namespace, with no
    /// requirement for a schema to be present or for schema validation of the &lt;StatusDetail&gt; contents.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=Saml20Constants.PROTOCOL)]
    [XmlRoot(ElementName, Namespace=Saml20Constants.PROTOCOL, IsNullable=false)]
    public class StatusDetail
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "StatusDetail";

        #region Elements

        /// <summary>
        /// Gets or sets any.
        /// </summary>
        /// <value>Any.</value>
        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        #endregion
    }
}
