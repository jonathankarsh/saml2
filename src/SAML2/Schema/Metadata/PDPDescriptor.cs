using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;PDPDescriptor&gt; element extends RoleDescriptorType with content reflecting profiles specific to
    /// policy decision points, SAML authorities that respond to &lt;samlp:AuthzDecisionQuery&gt; messages.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class PDPDescriptor : RoleDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "Organization";

        #region Elements

        /// <summary>
        /// Gets or sets the authz service.
        /// One or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Authorization Decision Query protocol defined in [SAMLProf]. All policy decision points support
        /// at least one such endpoint, by definition.
        /// </summary>
        /// <value>The authz service.</value>
        [XmlElement("AuthzService")]
        public Endpoint[] AuthzService { get; set; }

        /// <summary>
        /// Gets or sets the assertion ID request service.
        /// Zero or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Assertion Request protocol defined in [SAMLProf] or the special URI binding for assertion
        /// requests defined in [SAMLBind].
        /// </summary>
        /// <value>The assertion ID request service.</value>
        [XmlElement("AssertionIDRequestService")]
        public Endpoint[] AssertionIDRequestService { get; set; }

        /// <summary>
        /// Gets or sets the name ID format.
        /// Zero or more elements of type anyURI that enumerate the name identifier formats supported by
        /// this authority.
        /// </summary>
        /// <value>The name ID format.</value>
        [XmlElement("NameIDFormat", DataType="anyURI")]
        public string[] NameIDFormat { get; set; }

        #endregion
    }
}
