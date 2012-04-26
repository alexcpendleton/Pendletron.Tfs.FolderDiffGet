﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;
using System.Reflection;
using Pendletron.Tfs.FolderDiffGet.Core.Wrappers;
using Pendletron.Vsix.Core.Wrappers;
using Pendletron.Tfs.FolderDiffGet.Core;
using Microsoft.TeamFoundation.Client;

namespace Pendletron.Pendletron_Tfs_FolderDiffGet_Vsix
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidPendletron_Tfs_FolderDiffGet_VsixPkgString)]
    public sealed class Pendletron_Tfs_FolderDiffGet_VsixPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public Pendletron_Tfs_FolderDiffGet_VsixPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidPendletron_Tfs_FolderDiffGet_VsixCmdSet, (int)PkgCmdIDList.cmdidGetFolderDiff);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
			GetActiveDocument();
        }

		private DTE2 _dteInstance = null;
		public DTE2 DTEInstance {
			get {
				if (_dteInstance == null) {
					_dteInstance = GetDTEService();
				}
				return _dteInstance;
			}
			set { _dteInstance = value; }
		}

		private DTE2 GetDTEService() {
			return (DTE2)GetService(typeof(DTE));
		}

    	private Assembly _vsTfsVcAssembly;
    	public Assembly VsTfsVcAssembly
    	{
			get { return _vsTfsVcAssembly ?? (_vsTfsVcAssembly = GetVsTfsVcAssembly()); }
			set { _vsTfsVcAssembly = value; }
    	}

		private Assembly GetVsTfsVcAssembly()
		{
			var a = Assembly.Load("Microsoft.VisualStudio.TeamFoundation.VersionControl");
			return a;
		}

		public void GetActiveDocument()
		{
			var doc = DTEInstance.ActiveDocument;
			Guid guid = new Guid("E3FC08BE-3924-11DB-8AF6-B622A1EF5492");
			var window = DTEInstance.ActiveWindow;
			if (window.ObjectKind.ToUpper().Contains(guid.ToString().ToUpper()))
			{
				var hatpack = new HatPackage();
				dynamic man = new AccessPrivateWrapper(hatpack._wrapped.FolderDiffManager);
				if (man != null)
				{
					dynamic toolWindow = new AccessPrivateWrapper(man.FolderDiffToolWindows[0]);
					if(toolWindow != null)
					{
						dynamic diffControl = new AccessPrivateWrapper(toolWindow.FolderDiffControl);
						if(diffControl != null)
						{
							var diff = diffControl.FolderDiff;
							if (diff != null) {
								var coll = hatpack.HatterasService.TeamProjectCollection as TfsTeamProjectCollection;
								string outputDir = @"D:\FolderDiffGet.ConsoleOutput";
								var getter = new FolderDiffInternalsGetter(diff, coll, outputDir);
								getter.Go();
								
							}
						}
					}
				}
			}
			//((Microsoft.TeamFoundation.VersionControl.Controls.ControlFolderDiffDisplay)(hatpack._wrapped.FolderDiffManager.FolderDiffToolWindows[0].FolderDiffControl)).FolderDiff

			
				

			}
			/*
namespace Microsoft.VisualStudio.TeamFoundation.VersionControl
{
	[Guid("E3FC08BE-3924-11DB-8AF6-B622A1EF5492")]
	internal class ToolWindowFolderDiff : ToolWindowPaneBase, IOleCommandTarget
	{*/

		}

    }
