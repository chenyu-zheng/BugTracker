using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;

namespace BugTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create Uploads folder
            System.IO.Directory.CreateDirectory(Server.MapPath("~/Uploads"));

            // Generate default private.config template
            if (!System.IO.File.Exists(Server.MapPath("~/private.config")))
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  "
                };
                using (XmlWriter writer = XmlWriter.Create(Server.MapPath("~/private.config"), settings))
                {
                    writer.WriteStartElement("appSettings");

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "username");
                    writer.WriteAttributeString("value", "[smtp username]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "password");
                    writer.WriteAttributeString("value", "[smtp password]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "emailfrom");
                    writer.WriteAttributeString("value", "[from email address]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "emailto");
                    writer.WriteAttributeString("value", "[to email address]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "host");
                    writer.WriteAttributeString("value", "[smtp host]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "port");
                    writer.WriteAttributeString("value", "[smtp port]");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.Close();
                }
            }
        }
    }
}
