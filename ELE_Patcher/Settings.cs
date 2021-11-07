using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace ELE_Patcher
{
	public record Settings
	{
		[Tooltip("Most ELE conflicts can be solved by putting ELE late in the load order. Select this to skip patching records from ELE_SSE.esp, can save some time.")]
		public bool SkipVanilla = false;
	}
}
