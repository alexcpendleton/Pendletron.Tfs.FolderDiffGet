using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pendletron.Tfs.FolderDiffGet.Core.Wrappers;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections;
using System.Dynamic;
using Pendletron.Tfs.Core.Wrappers;

namespace Pendletron.Tfs.FolderDiffGet.Core.Wrappers {
	public class FolderDiffWrapper : IEnumerable
	{
		public dynamic _folderDiff;
		private Assembly _vcControlsAssembly;

		protected Type _progressUpdateCallbackType;
		protected Type _isCanceledCallbackType;
		protected Type _phaseChangedCallbackType;
		protected Type _folderDiffElementStatType;
		
		public const string FolderDiffTypeName = "Microsoft.TeamFoundation.VersionControl.Controls.FolderDiff";
		public const string VersionControlControlsAssemblyName = "Microsoft.TeamFoundation.VersionControl.Controls";
		public const string FolderDiffElementStateTypeName = VersionControlControlsAssemblyName + ".FolderDiffElementState";
		
		public bool ViewDifferent { get; set; }
		public bool ViewSame { get; set; }
		public bool ViewSourceOnly { get; set; }
		public bool ViewTargetOnly { get; set; }

		public static Assembly LoadVersonControlControlsAssemblyFromApplication()
		{
			return Assembly.Load(VersionControlControlsAssemblyName);
		}

		public FolderDiffWrapper(object folderDiffObject):this(folderDiffObject, LoadVersonControlControlsAssemblyFromApplication())
		{
			
		}

		public FolderDiffWrapper(object folderDiffObject, Assembly vcControlsAssembly)
		{
			_folderDiff = new AccessPrivateWrapper(folderDiffObject);
			_vcControlsAssembly = vcControlsAssembly;
			SetupTypesFromAssembly();
		}

		public FolderDiffWrapper(string assemblyPath, string srcPath, VersionSpec srcSpec, string targetPath, VersionSpec targetSpec, VersionControlServer server, RecursionType recursion)
		{
			_vcControlsAssembly = Assembly.LoadFrom(assemblyPath);
			//internal FolderDiff(string path1, VersionSpec spec1, string path2, VersionSpec spec2, VersionControlServer server, RecursionType recursion);
			_folderDiff = AccessPrivateWrapper.FromType(_vcControlsAssembly, FolderDiffTypeName,
			                                         srcPath, srcSpec, targetPath, targetSpec, server, recursion);
			SetupTypesFromAssembly();
		}

		protected virtual void SetupTypesFromAssembly()
		{
			_progressUpdateCallbackType = _vcControlsAssembly.GetType(FolderDiffTypeName + "+ProgressUpdateCallback");
			_isCanceledCallbackType = _vcControlsAssembly.GetType(FolderDiffTypeName + "+IsCanceledCallback");
			_phaseChangedCallbackType = _vcControlsAssembly.GetType(FolderDiffTypeName + "+PhaseChangedCallback");
			_folderDiffElementStatType = _vcControlsAssembly.GetType(FolderDiffElementStateTypeName);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			//internal void Initialize(ProgressUpdateCallback progressCallback, IsCanceledCallback isCanceledCallback, PhaseChangedCallback phaseChangedCallback)
			var types = new Type[]
			            {
							_progressUpdateCallbackType, 
							_isCanceledCallbackType,
							_phaseChangedCallbackType
			            };

			var nullProg = Convert.ChangeType(null, _progressUpdateCallbackType);
			var nullIsCanceled = Convert.ChangeType(null, _isCanceledCallbackType);
			var nullPhase = Convert.ChangeType(null, _phaseChangedCallbackType);

			//x.Initialize(nullProg, nullIsCanceled, nullPhase);
			var initMethod = _folderDiff._wrapped.GetType().GetMethod("Initialize", AccessPrivateWrapper.flags, null, types, null);
			initMethod.Invoke(_folderDiff._wrapped, new[] { nullProg, nullIsCanceled, nullPhase });
			bool? filterLocalPathsOnly = false;
			string filter = "";
			var view = MakeViewEnum();
			dynamic options = new AccessPrivateWrapper(_folderDiff.Options);
			options.UseRegistryDefaults = false;
			if (view != null)
			{
				options.ViewOptions = view;
			}
			_folderDiff.Compare();
			return _folderDiff.GetEnumerator();
		}

		public virtual object MakeViewEnum()
		{
			FolderDiffElementStateWrapper w = FolderDiffElementStateWrapper.None;
			if(ViewDifferent)
			{
				w |= FolderDiffElementStateWrapper.BothPathsDifferent;
			}
			if(ViewSame)
			{
				w |= FolderDiffElementStateWrapper.BothPathsSame;
			}
			if(ViewSourceOnly)
			{
				w |= FolderDiffElementStateWrapper.Path1Only; 
			}
			if(ViewTargetOnly)
			{
				w |= FolderDiffElementStateWrapper.Path2Only;
			}
			object toParse = w.ToString();
			return Convert.ChangeType(Enum.Parse(_folderDiffElementStatType, toParse.ToString()), _folderDiffElementStatType);
		}

		public string Path1 { get { return _folderDiff.m_path1; } set { _folderDiff.m_path1 = value; } }
		public string Path2 { get { return _folderDiff.m_path2; } set { _folderDiff.m_path2 = value; } }

	}
}
