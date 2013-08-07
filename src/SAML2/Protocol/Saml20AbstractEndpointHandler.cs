using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using SAML2.Bindings;
using SAML2.Config;
using SAML2.Logging;
using SAML2.Properties;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Protocol
{
    /// <summary>
    /// Base class for all SAML20 specific endpoints.
    /// </summary>
    public abstract class Saml20AbstractEndpointHandler : AbstractEndpointHandler
    {

        /// <summary>
        /// Parameter name for idp choice
        /// </summary>
        public const string IDPChoiceParameterName = "cidp";
        /// <summary>
        /// Key used to save login session
        /// </summary>
        public const string IDPLoginSessionKey = "LoginIDPId";
        /// <summary>
        /// Key used to save temporary session id
        /// </summary>
        public const string IDPTempSessionKey = "TempIDPId";
        /// <summary>
        /// Key used to save SessionId
        /// </summary>
        public const string IDPSessionIdKey = "IDPSessionID";

        /// <summary>
        /// Key used to override ForceAuthn setting
        /// </summary>
        public const string IDPForceAuthn = "IDPForceAuthn";

        /// <summary>
        /// Key used to override IsPassive setting
        /// </summary>
        public const string IDPIsPassive = "IDPIsPassive";

        /// <summary>
        /// Used to save the name id format of the assertion
        /// </summary>
        public const string IDPNameIdFormat = "IDPNameIdFormat";

        /// <summary>
        /// Key used to save the idp name id in session context
        /// </summary>
        public const string IDPNameId = "IDPNameId";

        /// <summary>
        /// Determines if configuration has been validated
        /// </summary>
        public static bool validated = false;

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public sealed override void ProcessRequest(HttpContext context)
        {
            try
            {
                CheckConfiguration(context);
                Handle(context);
            }
            catch (Exception ex)
            {
                HandleError(context, ex);
            }
        }

        /// <summary>
        /// Checks the configuration elements and redirects to an error page if something is missing or wrong.
        /// </summary>
        /// <param name="ctx"></param>
        private void CheckConfiguration(HttpContext ctx)
        {
            if (validated)
                return;

            string errorMessage;
            validated = BindingUtility.ValidateConfiguration(out errorMessage);
            if (!validated)            
                HandleError(ctx, errorMessage);                        
        }

        /// <summary>
        /// Abstract handler function
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected abstract void Handle(HttpContext ctx);

        /// <summary>
        /// Handles the selection of an IDP. If only one IDP is found, the user is automatically redirected to it.
        /// If several are found, and nothing indicates to which one the user should be sent, this method returns null.
        /// </summary>
        public IdentityProviderElement RetrieveIDP(HttpContext context)
        {
            var config = Saml2Config.GetConfig();
            config.IdentityProviders.Refresh();

            //If idpChoice is set, use it value
            if (!string.IsNullOrEmpty(context.Request.Params[IDPChoiceParameterName]))
            {
                Logger.Debug("Using IDPChoiceParamater: " + context.Request.Params[IDPChoiceParameterName]);
                IdentityProviderElement endPoint = config.IdentityProviders.FirstOrDefault(x => x.Id == context.Request.Params[IDPChoiceParameterName]);
                if (endPoint != null)                
                    return endPoint;                
            }

            //If we have a common domain cookie, use it's value
            //It must have been returned from the local common domain cookie reader endpoint.
            if (!string.IsNullOrEmpty(context.Request.QueryString["_saml_idp"]))
            {
                CommonDomainCookie cdc = new CommonDomainCookie(context.Request.QueryString["_saml_idp"]);
                if (cdc.IsSet)
                {
                    IdentityProviderElement endPoint = config.IdentityProviders.FirstOrDefault(x => x.Id == cdc.PreferredIDP);
                    if (endPoint != null)
                    {
                        Logger.Debug("IDP read from Common Domain Cookie: " + cdc.PreferredIDP);
                    
                        return endPoint;
                    }

                    Logger.Warn("Invalid IdP in Common Domain Cookie, IdP not found in list of IdPs: " + cdc.PreferredIDP);
                }
            }

            //If there is only one configured IdentityProviderEndpointElement lets just use that
            if (config.IdentityProviders.Count == 1 && config.IdentityProviders[0].Metadata != null)
            {
                Logger.Debug("No IdP selected in Common Domain Cookie, using default IdP: " + config.IdentityProviders[0].Name);
                return config.IdentityProviders[0];
            }

            // If one of the endpoints are marked with default, use that one
            var defaultIdp = config.IdentityProviders.FirstOrDefault(idp => idp.Default);
            if(defaultIdp != null)
            {
                Logger.Debug("Using IdP marked as default: " + defaultIdp.Id);

                return defaultIdp;
            }

            // In case an Idp selection url has been configured, redirect to that one.
            if(!string.IsNullOrEmpty(config.IdentityProviders.SelectionUrl))
            {
                Logger.Debug("Redirecting to idpSelectionUrl for selection of IDP: " + config.IdentityProviders.SelectionUrl);

                context.Response.Redirect(config.IdentityProviders.SelectionUrl);
            }

            // If an IDPSelectionEvent handler is present, request the handler for an IDP endpoint to use.
            var idpEndpoint = IDPSelectionUtil.InvokeIDPSelectionEventHandler(config.IdentityProviders);
            if(idpEndpoint != null)
            {
                return idpEndpoint;
            }

            return null;
        }

        /// <summary>
        /// Looks through the Identity Provider configurations and 
        /// </summary>
        public IdentityProviderElement RetrieveIDPConfiguration(string IDPId)
        {
            var config = Saml2Config.GetConfig();
            config.IdentityProviders.Refresh();
            return config.IdentityProviders.FirstOrDefault(x => x.Id == IDPId);
        }

        /// <summary>
        /// Utility function for error handling.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="status">The status.</param>
        protected void HandleError(HttpContext context, Status status)
        {
            string errorMessage = string.Format("ErrorCode: {0}. Message: {1}.", status.StatusCode.Value, status.StatusMessage);

            if(status.StatusCode.SubStatusCode != null){
                switch (status.StatusCode.SubStatusCode.Value)
                {
                    case Saml20Constants.StatusCodes.AuthnFailed:
                        HandleError(context, errorMessage, true);
                        break;
                    default:
                        HandleError(context, errorMessage, false);
                        break;
                }
            } else {
                HandleError(context, errorMessage, false);
            }                
        }

        /// <summary>
        /// Determine which endpoint to use based on the protocol defaults, configuration data and metadata.
        /// </summary>
        /// <param name="defaultBinding">The binding to use if none has been specified in the configuration and the metadata allows all bindings.</param>
        /// <param name="config">The endpoint as described in the configuration. May be null.</param>
        /// <param name="metadata">A list of endpoints of the given type (eg. SSO or SLO) that the metadata contains. </param>        
        internal static IdentityProviderEndpointElement DetermineEndpointConfiguration(BindingType defaultBinding, IdentityProviderEndpointElement config, List<IdentityProviderEndpointElement> metadata)
        {
            IdentityProviderEndpointElement result = new IdentityProviderEndpointElement();
            result.Binding = defaultBinding;

            // Determine which binding to use.
            if (config != null)
            {
                result.Binding = config.Binding;
            } else {
                // Verify that the metadata allows the default binding.
                bool allowed = metadata.Exists(delegate(IdentityProviderEndpointElement el) { return el.Binding == defaultBinding; });
                if (!allowed)
                {
                    if (result.Binding == BindingType.Post)
                        result.Binding = BindingType.Redirect;
                    else
                        result.Binding = BindingType.Post;
                }                    
            }

            if (config != null && !string.IsNullOrEmpty(config.Url))
            {
                result.Url = config.Url;
            } else
            {
                IdentityProviderEndpointElement endpoint =
                    metadata.Find(delegate(IdentityProviderEndpointElement el) { return el.Binding == result.Binding; });

                if (endpoint == null)
                    throw new ConfigurationErrorsException(
                        String.Format("No IdentityProvider supporting SAML binding {0} found in metadata",
                                      result.Binding));

                result.Url = endpoint.Url;
            }

            return result;
        }

    }
}