﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewPlatform.Flexberry.ORM.ODataService.Tests {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NewPlatform.Flexberry.ORM.ODataService.Tests.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to CREATE TABLE [Driver] (
        ///
        ///	 [primaryKey] UNIQUEIDENTIFIER  NOT NULL,
        ///
        ///	 [Name] VARCHAR(255)  NULL,
        ///
        ///	 [CarCount] INT  NULL,
        ///
        ///	 [Documents] BIT  NULL,
        ///
        ///	 PRIMARY KEY ([primaryKey]))
        ///
        ///
        ///CREATE TABLE [Страна] (
        ///
        ///	 [primaryKey] UNIQUEIDENTIFIER  NOT NULL,
        ///
        ///	 [Название] VARCHAR(255)  NULL,
        ///
        ///	 PRIMARY KEY ([primaryKey]))
        ///
        ///
        ///CREATE TABLE [Master] (
        ///
        ///	 [primaryKey] UNIQUEIDENTIFIER  NOT NULL,
        ///
        ///	 [property] VARCHAR(255)  NULL,
        ///
        ///	 PRIMARY KEY ([primaryKey]))
        ///
        ///
        ///CREATE TABLE [Лес] (
        ///
        ///	 [pr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MssqlScript {
            get {
                return ResourceManager.GetString("MssqlScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///
        ///
        ///
        ///
        ///
        ///CREATE TABLE &quot;TestDetailWithCicle&quot; (
        ///
        /// &quot;primaryKey&quot; RAW(16) NOT NULL,
        ///
        /// &quot;TestDetailName&quot; NVARCHAR2(255) NULL,
        ///
        /// &quot;Parent&quot; RAW(16) NULL,
        ///
        /// &quot;TestMaster&quot; RAW(16) NOT NULL,
        ///
        /// PRIMARY KEY (&quot;primaryKey&quot;)
        ///
        /// );
        ///
        ///CREATE TABLE &quot;TestMaster&quot;
        ///(
        ///
        ///	 &quot;primaryKey&quot; RAW(16) NOT NULL,
        ///
        ///	 &quot;TestMasterName&quot; NVARCHAR2(255) NULL,
        ///
        ///	 PRIMARY KEY (&quot;primaryKey&quot;)
        ///
        /// );
        ///
        ///CREATE TABLE &quot;Детейл2&quot;
        ///(
        ///
        ///	&quot;primaryKey&quot; RAW(16) NOT NULL,
        ///
        ///	&quot;prop2&quot; NVARCHAR2(255) NULL,
        ///
        ///	&quot;Детейл&quot; RAW(16) NOT NULL,
        ///
        ///	 PRI [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string OracleScript {
            get {
                return ResourceManager.GetString("OracleScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE Driver (
        ///
        /// primaryKey UUID NOT NULL,
        ///
        /// Name VARCHAR(255) NULL,
        ///
        /// CarCount INT NULL,
        ///
        /// Documents BOOLEAN NULL,
        ///
        /// PRIMARY KEY (primaryKey));
        ///
        ///
        ///CREATE TABLE Страна (
        ///
        /// primaryKey UUID NOT NULL,
        ///
        /// Название VARCHAR(255) NULL,
        ///
        /// PRIMARY KEY (primaryKey));
        ///
        ///
        ///CREATE TABLE Master (
        ///
        /// primaryKey UUID NOT NULL,
        ///
        /// property VARCHAR(255) NULL,
        ///
        /// PRIMARY KEY (primaryKey));
        ///
        ///
        ///CREATE TABLE Лес (
        ///
        /// primaryKey UUID NOT NULL,
        ///
        /// Название VARCHAR(255) NULL,
        ///
        /// Площадь INT NULL,
        ///        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PostgresScript {
            get {
                return ResourceManager.GetString("PostgresScript", resourceCulture);
            }
        }
    }
}
