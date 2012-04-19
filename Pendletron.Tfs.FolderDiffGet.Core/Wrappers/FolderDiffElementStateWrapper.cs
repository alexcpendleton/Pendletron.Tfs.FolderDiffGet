using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pendletron.Tfs.Core.Wrappers {
	public enum FolderDiffElementStateWrapper {
		None = 0,
		Path1Only = 1,
		Path2Only = 2,
		BothPathsSame = 4,
		BothPathsDifferent = 8,
		All = BothPathsDifferent | BothPathsSame | Path2Only | Path1Only,
	}
}
