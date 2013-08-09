using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The recipient of an &lt;ArtifactResolve&gt; message MUST respond with an &lt;ArtifactResponse&gt;
    /// message element. This element is of complex type ArtifactResponseType, which extends
    /// StatusResponseType with a single optional wildcard element corresponding to the SAML protocol
    /// message being returned. This wrapped message element can be a request or a response.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=Saml20Constants.PROTOCOL)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.PROTOCOL, IsNullable = false)]
    public class ArtifactResponse : StatusResponse
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "ArtifactResponse";

        #region Elements

        /// <summary>
        /// Gets or sets any.
        /// </summary>
        /// <value>Any.</value>
        [XmlAnyElement]
        public XmlElement Any { get; set; }

        #endregion
    }
}
