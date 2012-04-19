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
		private dynamic _wrapper;
		private Assembly _vcControlsAssembly;
		public FolderDiffWrapper(string assemblyPath, string srcPath, VersionSpec srcSpec, string targetPath, VersionSpec targetSpec, VersionControlServer server, RecursionType recursion)
		{/*
			_vcAssembly = Assembly.Load("Microsoft.VisualStudio.TeamFoundation.VersionControl");
			Type t = _vcAssembly.GetType("Microsoft.VisualStudio.TeamFoundation.VersionControl.HatPackage");
			var prop = t.GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static);
			object instance = prop.GetValue(null, null);
			_wrapped = new AccessPrivateWrapper(instance);
		  * */
			_vcControlsAssembly = Assembly.LoadFrom(assemblyPath);
			//Type t = _vcControlsAssembly.GetType("Microsoft.TeamFoundation.VersionControl.Controls.FolderDiff");
			string typeName = "Microsoft.TeamFoundation.VersionControl.Controls.FolderDiff";
			//internal FolderDiff(string path1, VersionSpec spec1, string path2, VersionSpec spec2, VersionControlServer server, RecursionType recursion);
			_wrapper = AccessPrivateWrapper.FromType(_vcControlsAssembly, typeName,
			                                         srcPath, srcSpec, targetPath, targetSpec, server, recursion);
			_progressUpdateCallbackType = _vcControlsAssembly.GetType(typeName + "+ProgressUpdateCallback");
			_isCanceledCallbackType = _vcControlsAssembly.GetType(typeName + "+IsCanceledCallback");
			_phaseChangedCallbackType = _vcControlsAssembly.GetType(typeName + "+PhaseChangedCallback");
			_folderDiffElementStatType = _vcControlsAssembly.GetType("Microsoft.TeamFoundation.VersionControl.Controls.FolderDiffElementState");
		}

		private Type _progressUpdateCallbackType;
		private Type _isCanceledCallbackType;
		private Type _phaseChangedCallbackType;
		private Type _folderDiffElementStatType;

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			dynamic x = _wrapper;
			//internal void Initialize(ProgressUpdateCallback progressCallback, IsCanceledCallback isCanceledCallback, PhaseChangedCallback phaseChangedCallback)
			var types = new Type[]
			            {
							_progressUpdateCallbackType, 
							_isCanceledCallbackType,
							_phaseChangedCallbackType
			            };
			var o = new object();
			var ip = new IntPtr();
			//var nullProg = Delegate.CreateDelegate(_progressUpdateCallbackType,) // Activator.CreateInstance(_progressUpdateCallbackType, o, ip);
			var nullProg = Convert.ChangeType(null, _progressUpdateCallbackType);
			var nullIsCanceled = Convert.ChangeType(null, _isCanceledCallbackType);// Activator.CreateInstance(_isCanceledCallbackType, o, ip);
			var nullPhase = Convert.ChangeType(null, _phaseChangedCallbackType);// Activator.CreateInstance(_phaseChangedCallbackType, o, ip);
			//x.Initialize(nullProg, nullIsCanceled, nullPhase);

			
			var initMethod = _wrapper._wrapped.GetType().GetMethod("Initialize", AccessPrivateWrapper.flags, null, types, null);
			initMethod.Invoke(_wrapper._wrapped, new[] { nullProg, nullIsCanceled, nullPhase });
			bool? filterLocalPathsOnly = false;
			string filter = "";
			var view = MakeViewEnum();
			
			/*
			 * */
			/*
      FolderDiffOptions options = this.m_folderDiff.Options;
      options.UseRegistryDefaults = false;
      if (filter != null)
        options.FilterHistory = new List<string>()
        {
          filter
        };
      if (filterLocalPathsOnly.HasValue && filterLocalPathsOnly.HasValue)
        options.FilterLocalPathsOnly = filterLocalPathsOnly.Value;
      if (!view.HasValue || !view.HasValue)
        return;
      options.ViewOptions = view.Value;*/
			dynamic options = new AccessPrivateWrapper(_wrapper.Options);
			options.UseRegistryDefaults = false;
			if (view != null)
			{
				options.ViewOptions = view;
			}
			//x.InitializeOption(filter, filterLocalPathsOnly, view);
			x.Compare();


			var e = x.GetEnumerator();
			return e ?? null;
		}

		public bool ViewDifferent { get; set; }
		public bool ViewSame { get; set; }
		public bool ViewSourceOnly { get; set; }
		public bool ViewTargetOnly { get; set; }

		public object MakeViewEnum()
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
	}
}
