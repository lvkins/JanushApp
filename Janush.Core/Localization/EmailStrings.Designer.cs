﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Janush.Core.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class EmailStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Janush.Core.Localization.EmailStrings", typeof(EmailStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Product name has has been changed from {0} to &lt;strong&gt;{1}&lt;/strong&gt;
        ///&lt;hr&gt;
        ///Product: &lt;a href=&quot;{2}&quot;&gt;{2}&lt;/a&gt;.
        /// </summary>
        public static string NotificationNameChanged {
            get {
                return ResourceManager.GetString("NotificationNameChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The price of &lt;strong&gt;{0}&lt;/strong&gt; has been decreased from {1} to &lt;strong&gt;{2}&lt;/strong&gt;
        ///&lt;hr&gt;
        ///Product: &lt;a href=&quot;{3}&quot;&gt;{3}&lt;/a&gt;.
        /// </summary>
        public static string NotificationPriceDecrease {
            get {
                return ResourceManager.GetString("NotificationPriceDecrease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The price of &lt;strong&gt;{0}&lt;/strong&gt; has been increased from {1} to &lt;strong&gt;{2}&lt;/strong&gt;
        ///&lt;hr&gt;
        ///Product: &lt;a href=&quot;{3}&quot;&gt;{3}&lt;/a&gt;.
        /// </summary>
        public static string NotificationPriceIncrease {
            get {
                return ResourceManager.GetString("NotificationPriceIncrease", resourceCulture);
            }
        }
    }
}
