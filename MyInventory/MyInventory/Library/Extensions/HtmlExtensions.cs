using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MyInventory.Library.Extensions
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            string description = metadata.Description;

            return MvcHtmlString.Create(string.IsNullOrWhiteSpace(description) ? "" : string.Format(@"<small class=""d-block"">{0}</small>", description));
        }

        public class HtmlBlock : IDisposable
        {
            private const string BLOCK_KEY = "_htmlblock_";
            public static Dictionary<string, string> Blocks
            {
                get
                {
                    if (HttpContext.Current.Items[BLOCK_KEY] == null)
                        HttpContext.Current.Items[BLOCK_KEY] = new Dictionary<string, string>();
                    return (Dictionary<string, string>)HttpContext.Current.Items[BLOCK_KEY];
                }
            }

            private WebViewPage webViewPage;
            private string blockName;

            public HtmlBlock(WebViewPage webViewPage, string blockName)
            {
                this.blockName = blockName;
                this.webViewPage = webViewPage;
                this.webViewPage.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                if (Blocks.ContainsKey(blockName))
                {
                    Blocks[blockName] += ((StringWriter)webViewPage.OutputStack.Pop()).ToString();
                }
                else
                {
                    Blocks.Add(blockName, ((StringWriter)webViewPage.OutputStack.Pop()).ToString());
                }
            }
        }

        public static IDisposable BeginHtmlBlock(this HtmlHelper helper, string blockName)
        {
            return new HtmlBlock((WebViewPage)helper.ViewDataContainer, blockName);
        }

        public static MvcHtmlString RenderHtmlBlock(this HtmlHelper helper, string blockName)
        {
            if (HtmlBlock.Blocks.ContainsKey(blockName))
            {
                return MvcHtmlString.Create(HtmlBlock.Blocks[blockName]);
            }
            else
            {
                return MvcHtmlString.Create(String.Empty);
            }
        }

    }
    
}