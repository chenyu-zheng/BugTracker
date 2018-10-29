using Ganss.XSS;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.HtmlHelperExtensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString RawSafe(this HtmlHelper helper, string value)
        {
            var sanitizer = new HtmlSanitizer();
            return MvcHtmlString.Create(sanitizer.Sanitize(value));
        }
    }
}