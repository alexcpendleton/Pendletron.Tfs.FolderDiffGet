using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

public class OutputWindowTraceListener : TraceListener {
	public OutputWindowTraceListener(IVsOutputWindowPane pane) {
		OutputPane = pane;
	}
	/*
	 * IVsOutputWindow outWindow = Package.GetGlobalService( typeof( SVsOutputWindow ) ) as IVsOutputWindow;

Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane; // P.S. There's also the GUID_OutWindowDebugPane available.
IVsOutputWindowPane generalPane;
outWindow.GetPane( ref generalPaneGuid , out generalPane );

generalPane.OutputString( "Hello World!" );
generalPane.Activate(); // Brings this pane into view*/
	public IVsOutputWindowPane OutputPane { get; set; }
	public static IVsOutputWindowPane FindGeneralPane(IVsOutputWindow outputWindow) {
		Guid paneGuid = VSConstants.GUID_OutWindowGeneralPane;
		IVsOutputWindowPane results;
		outputWindow.GetPane(ref paneGuid, out results);
		return results;
	}

	public override void Write(string message) {
		OutputPane.OutputString(message);
	}

	public override void WriteLine(string message) {
		Write(Environment.NewLine);
		Write(message);
	}
}