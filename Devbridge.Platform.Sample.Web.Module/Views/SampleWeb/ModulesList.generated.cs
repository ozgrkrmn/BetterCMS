﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Devbridge.Platform.Sample.Web.Module.Views.SampleWeb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/SampleWeb/ModulesList.cshtml")]
    public partial class ModulesList : System.Web.Mvc.WebViewPage<Devbridge.Platform.Sample.Web.Module.Models.ModulesListViewModel>
    {
        public ModulesList()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\SampleWeb\ModulesList.cshtml"
 if (Model == null)
{

            
            #line default
            #line hidden
WriteLiteral("    <h1>Modules list model is null</h1>\r\n");

            
            #line 6 "..\..\Views\SampleWeb\ModulesList.cshtml"
}
else
{
    if (Model.Modules == null || Model.Modules.Count == 0)
    {

            
            #line default
            #line hidden
WriteLiteral("        <h1>Modules list is empty</h1>\r\n");

            
            #line 12 "..\..\Views\SampleWeb\ModulesList.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <h1>The list of references modules:</h1>\r\n");

            
            #line 16 "..\..\Views\SampleWeb\ModulesList.cshtml"
    

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" border=\"1\"");

WriteLiteral(">\r\n            <tr>\r\n                <th>Assembly</th>\r\n                <th>Name<" +
"/th>\r\n                <th>Description</th>\r\n                <th>Descriptor Type<" +
"/th>\r\n            </tr>\r\n");

            
            #line 24 "..\..\Views\SampleWeb\ModulesList.cshtml"
            
            
            #line default
            #line hidden
            
            #line 24 "..\..\Views\SampleWeb\ModulesList.cshtml"
             foreach (var module in Model.Modules)
            {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td>");

            
            #line 27 "..\..\Views\SampleWeb\ModulesList.cshtml"
                   Write(module.Assembly);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td>");

            
            #line 28 "..\..\Views\SampleWeb\ModulesList.cshtml"
                   Write(module.Name);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td>");

            
            #line 29 "..\..\Views\SampleWeb\ModulesList.cshtml"
                   Write(module.Description);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                    <td>");

            
            #line 30 "..\..\Views\SampleWeb\ModulesList.cshtml"
                   Write(module.Type);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                </tr>\r\n");

            
            #line 32 "..\..\Views\SampleWeb\ModulesList.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </table>\r\n");

            
            #line 34 "..\..\Views\SampleWeb\ModulesList.cshtml"
    }
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
