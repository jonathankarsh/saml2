using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using SAML2.Config;
using Saml2.Properties;
using System.Security.Cryptography.Xml;

namespace SAML2.Protocol
{
    /// <summary>
    /// The handler that exposes a metadata endpoint to the other parties of the federation.
    ///     
    /// The handler accepts the following GET parameters :
    /// - encoding : Delivers the Metadata document in the specified encoding. Example: encoding=iso-8859-1 . If the parameter is omitted, the encoding utf-8 is used.
    /// - sign : A boolean parameter specifying whether to sign the metadata document. Example: sign=false. If the parameter is omitted, the document is signed.
    /// </summary>
    public class Saml20MetadataHandler : AbstractEndpointHandler
    {
        #region IHttpHandler Members

        /// <summary>
        /// Gets a value indicating whether this instance is reusable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is reusable; otherwise, <c>false</c>.
        /// </value>
        public new bool IsReusable
        {
            get { return false; }
        }
        
        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public override void ProcessRequest(HttpContext context)
        {
            var encoding = context.Request.QueryString["encoding"];
            try
            {
                if (!string.IsNullOrEmpty(encoding))
                {
                    context.Response.ContentEncoding = Encoding.GetEncoding(encoding);
                }
            }
            catch (ArgumentException)
            {
                Logger.Error(Resources.UnknownEncodingFormat(encoding));
                HandleError(context, Resources.UnknownEncodingFormat(encoding));
                return;
            }

            var sign = true;
            try
            {
                var param = context.Request.QueryString["sign"];                
                if (!string.IsNullOrEmpty(param))
                {
                    sign = Convert.ToBoolean(param);
                }
            }
            catch(FormatException)
            {
                Logger.Error(Resources.GenericError);
                HandleError(context, Resources.GenericError);
                return;
            }
                        
            context.Response.ContentType = Saml20Constants.MetadataMimetype;
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"metadata.xml\"");

            CreateMetadataDocument(context, sign);
            
            context.Response.End();            
        }

        #endregion

        /// <summary>
        /// Creates the metadata document.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sign">if set to <c>true</c> sign the document.</param>
        private void CreateMetadataDocument(HttpContext context, bool sign)
        {
            Logger.Debug("Creating metadata document.");

            var configuration = Saml2Config.GetConfig();

            var keyinfo = new KeyInfo();
            var keyClause = new KeyInfoX509Data(Saml2Config.GetConfig().ServiceProvider.SigningCertificate.GetCertificate(), X509IncludeOption.EndCertOnly);
            keyinfo.AddClause(keyClause);

            var doc = new Saml20MetadataDocument(configuration, keyinfo, sign);

            Logger.Debug("Metadata document successfully created.");

            context.Response.Write(doc.ToXml( context.Response.ContentEncoding ));
        }
    }
}
