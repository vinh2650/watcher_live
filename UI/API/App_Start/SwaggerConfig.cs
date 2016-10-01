using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing.Constraints;
using WebActivatorEx;
using API;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]
namespace API
{
    /// <summary>
    /// swagger configuration
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// register swagger
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        // By default, the service root url is inferred from the request used to access the docs.
                        // However, there may be situations (e.g. proxy and load-balanced environments) where this does not
                        // resolve correctly. You can workaround this by providing your own code to determine the root URL.
                        //
                        //c.RootUrl(req => GetRootUrlFromAppConfig());

                        // If schemes are not explicitly provided in a Swagger 2.0 document, then the scheme used to access
                        // the docs is taken as the default. If your API supports multiple schemes and you want to be explicit
                        // about them, you can use the "Schemes" option as shown below.
                        //
                        //c.Schemes(new[] { "http", "https" });

                        // Use "SingleApiVersion" to describe a single version API. Swagger 2.0 includes an "Info" object to
                        // hold additional metadata for an API. Version and title are required but you can also provide
                        // additional fields by chaining methods off SingleApiVersion.
                        //
                       
                        // c.SingleApiVersion("v1", "API for AMS Solution");

                        c.MultipleApiVersions(
                         (apiDesc, targetApiVersion) => ResolveVersionSupportByRouteConstraint(apiDesc, targetApiVersion),
                         (vc) =>
                         {

                             //vc.Version("V3", "AMS API V3").Contact(cc => cc
                             //  .Name("Insightus")
                             //  .Url("https://insightus.com.au")
                             //  .Email("support@insightus.com.au"));

                             //vc.Version("V2", "AMS API V2").Contact(cc => cc
                             //   .Name("Insightus")
                             //   .Url("https://insightus.com.au")
                             //   .Email("support@insightus.com.au"));
                             vc.Version("V1", "WATCHER API V1").Contact(cc => cc
                                .Name("watcher"));

                         });
                        // If your API has multiple versions, use "MultipleApiVersions" instead of "SingleApiVersion".
                        // In this case, you must provide a lambda that tells Swashbuckle which actions should be
                        // included in the docs for a given API version. Like "SingleApiVersion", each call to "Version"
                        // returns an "Info" builder so you can provide additional metadata per API version.
                        //
                        //c.MultipleApiVersions(
                        //    (apiDesc, targetApiVersion) => ResolveVersionSupportByRouteConstraint(apiDesc, targetApiVersion),
                        //    (vc) =>
                        //    {
                        //        vc.Version("v2", "Swashbuckle Dummy API V2");
                        //        vc.Version("v1", "Swashbuckle Dummy API V1");
                        //    });

                        // You can use "BasicAuth", "ApiKey" or "OAuth2" options to describe security schemes for the API.
                        // See https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md for more details.
                        // NOTE: These only define the schemes and need to be coupled with a corresponding "security" property
                        // at the document or operation level to indicate which schemes are required for an operation. To do this,
                        // you'll need to implement a custom IDocumentFilter and/or IOperationFilter to set these properties
                        // according to your specific authorization implementation
                        //
                        //c.BasicAuth("basic")
                        //    .Description("Basic HTTP Authentication");
                        //
                        //c.ApiKey("apiKey")
                        //    .Description("API Key Authentication")
                        //    .Name("apiKey")
                        //    .In("header");
                        //
                        //c.OAuth2("oauth2")
                        //    .Description("OAuth2 Implicit Grant")
                        //    .Flow("implicit")
                        //    .AuthorizationUrl("http://petstore.swagger.wordnik.com/api/oauth/dialog")
                        //    //.TokenUrl("https://tempuri.org/token")
                        //    .Scopes(scopes =>
                        //    {
                        //        scopes.Add("read", "Read access to protected resources");
                        //        scopes.Add("write", "Write access to protected resources");
                        //    });

                        // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                        //c.IgnoreObsoleteActions();

                        // Each operation be assigned one or more tags which are then used by consumers for various reasons.
                        // For example, the swagger-ui groups operations according to the first tag of each operation.
                        // By default, this will be controller name but you can use the "GroupActionsBy" option to
                        // override with any value.
                        //
                        c.GroupActionsBy(GroupByFriendlyControllerNameFunc);

                        // You can also specify a custom sort order for groups (as defined by "GroupActionsBy") to dictate
                        // the order in which operations are listed. For example, if the default grouping is in place
                        // (controller name) and you specify a descending alphabetic sort order, then actions from a
                        // ProductsController will be listed before those from a CustomersController. This is typically
                        // used to customize the order of groupings in the swagger-ui.
                        //
                        //c.OrderActionGroupsBy(new DescendingAlphabeticComparer());

                        // If you annotate Controllers and API Types with
                        // Xml comments (http://msdn.microsoft.com/en-us/library/b2s063f7(v=vs.110).aspx), you can incorporate
                        // those comments into the generated docs and UI. You can enable this by providing the path to one or
                        // more Xml comment files.
                        //
                        //c.IncludeXmlComments(GetXmlCommentsPath());

                        // Swashbuckle makes a best attempt at generating Swagger compliant JSON schemas for the various types
                        // exposed in your API. However, there may be occasions when more control of the output is needed.
                        // This is supported through the "MapType" and "SchemaFilter" options:
                        //
                        // Use the "MapType" option to override the Schema generation for a specific type.
                        // It should be noted that the resulting Schema will be placed "inline" for any applicable Operations.
                        // While Swagger 2.0 supports inline definitions for "all" Schema types, the swagger-ui tool does not.
                        // It expects "complex" Schemas to be defined separately and referenced. For this reason, you should only
                        // use the "MapType" option when the resulting Schema is a primitive or array type. If you need to alter a
                        // complex Schema, use a Schema filter.
                        //
                        //c.MapType<ProductType>(() => new Schema { type = "integer", format = "int32" });

                        // If you want to post-modify "complex" Schemas once they've been generated, across the board or for a
                        // specific type, you can wire up one or more Schema filters.
                        //
                        //c.SchemaFilter<ApplySchemaVendorExtensions>();

                        // In a Swagger 2.0 document, complex types are typically declared globally and referenced by unique
                        // Schema Id. By default, Swashbuckle does NOT use the full type name in Schema Ids. In most cases, this
                        // works well because it prevents the "implementation detail" of type namespaces from leaking into your
                        // Swagger docs and UI. However, if you have multiple types in your API with the same class name, you'll
                        // need to opt out of this behavior to avoid Schema Id conflicts.
                        //
                        c.UseFullTypeNameInSchemaIds();

                        // Alternatively, you can provide your own custom strategy for inferring SchemaId's for
                        // describing "complex" types in your API.
                        //  
                        //c.SchemaId(t => t.FullName.Contains('`') ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);

                        // Set this flag to omit schema property descriptions for any type properties decorated with the
                        // Obsolete attribute 
                        //c.IgnoreObsoleteProperties();

                        // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                        // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                        // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                        // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                        // 
                        //c.DescribeAllEnumsAsStrings();

                        // Similar to Schema filters, Swashbuckle also supports Operation and Document filters:
                        //
                        // Post-modify Operation descriptions once they've been generated by wiring up one or more
                        // Operation filters.
                        //
                        //c.OperationFilter<AddDefaultResponse>();
                        //
                        // If you've defined an OAuth2 flow as described above, you could use a custom filter
                        // to inspect some attribute on each action and infer which (if any) OAuth2 scopes are required
                        // to execute the operation
                        //
                        //c.OperationFilter<AssignOAuth2SecurityRequirements>();

                        // Post-modify the entire Swagger document by wiring up one or more Document filters.
                        // This gives full control to modify the final SwaggerDocument. You should have a good understanding of
                        // the Swagger 2.0 spec. - https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md
                        // before using this option.
                        //
                        //c.DocumentFilter<ApplyDocumentVendorExtensions>();

                        // In contrast to WebApi, Swagger 2.0 does not include the query string component when mapping a URL
                        // to an action. As a result, Swashbuckle will raise an exception if it encounters multiple actions
                        // with the same path (sans query string) and HTTP method. You can workaround this by providing a
                        // custom strategy to pick a winner or merge the descriptions for the purposes of the Swagger docs 
                        //
                        //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                        // Wrap the default SwaggerGenerator with additional behavior (e.g. caching) or provide an
                        // alternative implementation for ISwaggerProvider with the CustomProvider option.
                        //
                        //c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                        c.IncludeXmlComments(GetXmlCommentsPath());
                        c.DocumentFilter<AuthTokenOperation>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        // Use the "InjectStylesheet" option to enrich the UI with one or more additional CSS stylesheets.
                        // The file must be included in your project as an "Embedded Resource", and then the resource's
                        // "Logical Name" is passed to the method as shown below.
                        //
                        //c.InjectStylesheet(containingAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testStyles1.css");

                        // Use the "InjectJavaScript" option to invoke one or more custom JavaScripts after the swagger-ui
                        // has loaded. The file must be included in your project as an "Embedded Resource", and then the resource's
                        // "Logical Name" is passed to the method as shown above.
                        //
                        //c.InjectJavaScript(thisAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testScript1.js");

                        c.InjectJavaScript(thisAssembly, "API.SwaggerXml.SwaggerUIEnableBearerToken.js");

                        // The swagger-ui renders boolean data types as a dropdown. By default, it provides "true" and "false"
                        // strings as the possible choices. You can use this option to change these to something else,
                        // for example 0 and 1.
                        //
                        //c.BooleanValues(new[] { "0", "1" });

                        // By default, swagger-ui will validate specs against swagger.io's online validator and display the result
                        // in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
                        // feature entirely.
                        //c.SetValidatorUrl("http://localhost/validator");
                        //c.DisableValidator();

                        // Use this option to control how the Operation listing is displayed.
                        // It can be set to "None" (default), "List" (shows operations for each resource),
                        // or "Full" (fully expanded: shows operations and their details).
                        //
                        //c.DocExpansion(DocExpansion.List);

                        // Use the CustomAsset option to provide your own version of assets used in the swagger-ui.
                        // It's typically used to instruct Swashbuckle to return your version instead of the default
                        // when a request is made for "index.html". As with all custom content, the file must be included
                        // in your project as an "Embedded Resource", and then the resource's "Logical Name" is passed to
                        // the method as shown below.
                        //
                        //c.CustomAsset("index", containingAssembly, "YourWebApiProject.SwaggerExtensions.index.html");

                        // If your API has multiple versions and you've applied the MultipleApiVersions setting
                        // as described above, you can also enable a select box in the swagger-ui, that displays
                        // a discovery URL for each version. This provides a convenient way for users to browse documentation
                        // for different API versions.
                        //
                        c.EnableDiscoveryUrlSelector();

                        // If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
                        // the Swagger 2.0 specification, you can enable UI support as shown below.
                        //
                        //c.EnableOAuth2Support("test-client-id", "test-realm", "Swagger UI");
                    });
        }

        /// <summary>
        /// get xml path comment
        /// </summary>
        /// <returns></returns>
        protected static string GetXmlCommentsPath()
        {
            return String.Format(@"{0}\SwaggerXml\AMS.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }

        private static bool ResolveVersionSupportByRouteConstraint(ApiDescription apiDesc, string targetApiVersion)
        {

            var controllerNamespace = apiDesc.ActionDescriptor.ControllerDescriptor.ControllerType.FullName;
            if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(controllerNamespace, string.Format("Controllers.{0}", targetApiVersion), CompareOptions.IgnoreCase) >= 0)
            {
                return true;
            }

            return false;
          
        }

       
        public static Func<ApiDescription, string> GroupByFriendlyControllerNameFunc
        {
            get
            {
                return apiDescription =>
                {
                    var controllerName = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName;

                    // Put in spaces
                    controllerName = Regex.Replace(controllerName, "([a-z]|[A-Z]{2,})([A-Z])", @"$1 $2");

                    controllerName = Regex.Replace(controllerName, @"\sv\d+", String.Empty, RegexOptions.IgnoreCase);

                    return controllerName;
                };
            }
        }
    }





    /// <summary>
    /// authentication token
    /// </summary>
    public class AuthTokenOperation : IDocumentFilter
    {
        /// <summary>
        /// apply method
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiExplorer"></param>
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/token", new PathItem
            {
                post = new Operation
                {
                    responses = new Dictionary<string, Response>()
                    {
                        { "400",new Response()
                            {
                                description = "Bad Request"
                            }
                        },
                        { "401",new Response()
                            {
                                description = "Invalid credential"
                            }
                        },
                        { "500",new Response()
                            {
                                description = "Internal Server Error"
                            }
                        },
                        { "200",new Response
                            {
                                description = "Success, return information of user session",
                                schema = new Schema()
                                {
                                    example = new LoginResult
                                            {
                                                access_token = "ve6oDw6vcKNAtBmjL9P6s4JTq1UKOlAB3p47",
                                                refresh_token = "8f3fda22-5e1a-49b4-a1a8-99fd90c01d5f",
                                                expires = "Sat, 28 Jan 2017 06:28:13 GMT",
                                                client_id = "0645c9dc-6433-4ca9-a00f-787008b80b0b",
                                                expires_in = 31535999,
                                                token_type = "bearer",
                                                userId="0c26e0f4-3f1d-4528-9f9d-99567a5f0b7e",
                                                username="quangvinh2650@gmail.com"

                                            },



                            } }
                        }
                    },

                    summary = "login",
                    tags = new List<string> { "Account" },
                    description = " For native device, add \"<span style = 'font-weight: bold;' > Authorization: Basic xxx </span > \" into Header of request.<br />"
                                + "For browser, add \"<span style='font-weight: bold;'>client_id</span>\" and \"<span style='font-weight: bold;'>client_secret</span>\" into body of request. <br />"
                                + "<span style='font-weight: bold;'>client_secret</span>: 79935f99-af74-48f0-8634-f4a47ffe426b  <br />"
                                + "<span style='font-weight: bold;'>client_id</span>: 6649049c-0285-4876-b69a-fe3d05c8d555 <br />"
                                + "<span style='font-weight: bold;'>basiccode</span>: NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg== ",
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter> {
                    new Parameter
                    {
                        type = "string",
                        name = "grant_type",
                        description = "alway be 'password'",
                        required = true,
                        @in = "formData",
                        @default = "password"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "username",
                        required = true,
                        description = "email address",
                        @in = "formData"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "password",
                        description = "password",
                        required = true,
                        @in = "formData"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "client_id",
                        description = "client_id",
                        required = true,
                        @in = "formData",
                        @default = "3421c917-7520-45c3-ab52-9bca75707a6d"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "client_secret",
                        description = "client_secret",
                        required = true,
                        @in = "formData",
                        @default = "mSezuSyxfnBgceEWhlmer62HHSDegEMCdVTosK/F0+I="
                    }
                }
                }
            });
        }
    }
    /// <summary>
    /// Login result
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// access token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double expires_in { get; set; }

        /// <summary>
        /// as:client_id
        /// </summary>
        public string client_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// .issued
        /// </summary>
        public string issued { get; set; }

        /// <summary>
        /// expires
        /// </summary>
        public string expires { get; set; }
    }
}
