﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cortex.Net.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cortex.Net.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The derivation ({0}) was already added to the set of observers of observable ({1}).
        /// </summary>
        internal static string AlreadyAddedObserverToObservable {
            get {
                return ResourceManager.GetString("AlreadyAddedObserverToObservable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BindDependencies expects a DependenciesState != ({0}).
        /// </summary>
        internal static string BindDependenciesExpectsStateNonEqual {
            get {
                return ResourceManager.GetString("BindDependenciesExpectsStateNonEqual", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only tracking IDerivation instances can be added with a DependenciesState != ({0}).
        /// </summary>
        internal static string CanOnlyAddTrackedDependencies {
            get {
                return ResourceManager.GetString("CanOnlyAddTrackedDependencies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Get expression is null for ({0}) of type ({1}).
        /// </summary>
        internal static string GetExpressionNull {
            get {
                return ResourceManager.GetString("GetExpressionNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Global unobservation should only be queued without observers..
        /// </summary>
        internal static string GlobalUnobservationOnlyWithoutObservers {
            get {
                return ResourceManager.GetString("GlobalUnobservationOnlyWithoutObservers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ({0}) is null..
        /// </summary>
        internal static string IsNull {
            get {
                return ResourceManager.GetString("IsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The derivation ({0}) does not exist in the set of observers of observable ({1}).
        /// </summary>
        internal static string ObserverNotInObservable {
            get {
                return ResourceManager.GetString("ObserverNotInObservable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ({0}) should only be called when the shared state is in batch mode..
        /// </summary>
        internal static string OnlyInBatch {
            get {
                return ResourceManager.GetString("OnlyInBatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ({0}) is read outside a reactive context..
        /// </summary>
        internal static string ReadOutsideReaction {
            get {
                return ResourceManager.GetString("ReadOutsideReaction", resourceCulture);
            }
        }
    }
}
