using System.Collections.Generic;
using System.Linq;

using Mutagen.Bethesda.Skyrim;

namespace ELE_Patcher.Utility
{
	public static class PatchField
	{
		#region Dummies
		static bool dummyBool = false;
		#endregion

		#region Applicable to multiple types
		public static void PatchRecordFlags<TMajor, TMajorGetter>(this TMajor patched, HashSet<TMajorGetter> vanillas, TMajorGetter modded, ref bool changed)
			where TMajor : SkyrimMajorRecord, TMajorGetter
			where TMajorGetter : ISkyrimMajorRecordGetter
		{
			var oldPatchedValue = patched.MajorRecordFlagsRaw;
			var moddedValue = modded.MajorRecordFlagsRaw;
			var vanillaValues = vanillas.Select(x => x.MajorRecordFlagsRaw);

			patched.MajorRecordFlagsRaw = oldPatchedValue.PatchRevertedFlags(vanillaValues, moddedValue, ref changed);
		}
		#endregion

		#region Image space specific
		public static void PatchHdr(this ImageSpace patched, HashSet<IImageSpaceGetter> vanillas, IImageSpaceGetter modded, ref bool changed)
		{
			var patchedValue = patched.Hdr;
			var moddedValue = modded.Hdr;
			var vanillaValues = vanillas.Select(x => x.Hdr);

			bool membersRevertedToVanilla = vanillaValues.Any(x =>
			{
				var reverted = new bool[9];
				var isVanilla = new bool[9];

				reverted[0] = Util.RevertedToVanilla(patchedValue?.EyeAdaptSpeed, x?.EyeAdaptSpeed, moddedValue?.EyeAdaptSpeed, ref isVanilla[0]);
				reverted[1] = Util.RevertedToVanilla(patchedValue?.BloomBlurRadius, x?.BloomBlurRadius, moddedValue?.BloomBlurRadius, ref isVanilla[1]);
				reverted[2] = Util.RevertedToVanilla(patchedValue?.BloomThreshold, x?.BloomThreshold, moddedValue?.BloomThreshold, ref isVanilla[2]);
				reverted[3] = Util.RevertedToVanilla(patchedValue?.BloomScale, x?.BloomScale, moddedValue?.BloomScale, ref isVanilla[3]);
				reverted[4] = Util.RevertedToVanilla(patchedValue?.ReceiveBloomThreshold, x?.ReceiveBloomThreshold, moddedValue?.ReceiveBloomThreshold, ref isVanilla[4]);
				reverted[5] = Util.RevertedToVanilla(patchedValue?.White, x?.White, moddedValue?.White, ref isVanilla[5]);
				reverted[6] = Util.RevertedToVanilla(patchedValue?.SunlightScale, x?.SunlightScale, moddedValue?.SunlightScale, ref isVanilla[6]);
				reverted[7] = Util.RevertedToVanilla(patchedValue?.SkyScale, x?.SkyScale, moddedValue?.SkyScale, ref isVanilla[7]);
				reverted[8] = Util.RevertedToVanilla(patchedValue?.EyeAdaptStrength, x?.EyeAdaptStrength, moddedValue?.EyeAdaptStrength, ref isVanilla[8]);

				return isVanilla.All(x => x) && reverted.Any(x => x);
			});

			if (membersRevertedToVanilla)
			{
				changed = true;
				patched.Hdr = moddedValue?.DeepCopy();
			}
		}
		public static void PatchCinematic(this ImageSpace patched, HashSet<IImageSpaceGetter> vanillas, IImageSpaceGetter modded, ref bool changed)
		{
			var patchedValue = patched.Cinematic;
			var moddedValue = modded.Cinematic;
			var vanillaValues = vanillas.Select(x => x.Cinematic);

			bool membersRevertedToVanilla = vanillaValues.Any(x =>
			{
				var reverted = new bool[3];
				var isVanilla = new bool[3];

				reverted[0] = Util.RevertedToVanilla(patchedValue?.Saturation, x?.Saturation, moddedValue?.Saturation, ref isVanilla[0]);
				reverted[1] = Util.RevertedToVanilla(patchedValue?.Brightness, x?.Brightness, moddedValue?.Brightness, ref isVanilla[1]);
				reverted[2] = Util.RevertedToVanilla(patchedValue?.Contrast, x?.Contrast, moddedValue?.Contrast, ref isVanilla[2]);

				return isVanilla.All(x => x) && reverted.Any(x => x);
			});

			if (membersRevertedToVanilla)
			{
				changed = true;
				patched.Cinematic = moddedValue?.DeepCopy();
			}
		}
		public static void PatchTint(this ImageSpace patched, HashSet<IImageSpaceGetter> vanillas, IImageSpaceGetter modded, ref bool changed)
		{
			var patchedValue = patched.Tint;
			var moddedValue = modded.Tint;
			var vanillaValues = vanillas.Select(x => x.Tint);

			bool membersRevertedToVanilla = vanillaValues.Any(x =>
			{
				var reverted = new bool[2];
				var isVanilla = new bool[2];

				reverted[0] = Util.RevertedToVanilla(patchedValue?.Amount, x?.Amount, moddedValue?.Amount, ref isVanilla[0]);
				reverted[1] = Util.RevertedToVanilla(patchedValue?.Color, x?.Color, moddedValue?.Color, ref isVanilla[1]);

				return isVanilla.All(x => x) && reverted.Any(x => x);
			});

			if (membersRevertedToVanilla)
			{
				changed = true;
				patched.Tint = moddedValue?.DeepCopy();
			}
		}

		#endregion
		#region Light specific
		public static void PatchFlags(this Light patched, HashSet<ILightGetter> vanillas, ILightGetter modded, ref bool changed)
		{
			var patchedValue = patched.Flags;
			var moddedValue = modded.Flags;
			var vanillaValues = vanillas.Select(x => x.Flags);

			patched.Flags = patchedValue.PatchRevertedFlags(vanillaValues, moddedValue, ref changed);
		}
		#endregion

		#region Cell specific
		public static void PatchFlags(this Cell patched, HashSet<ICellGetter> vanillas, ICellGetter modded, ref bool changed)
		{
			var patchedValue = patched.Flags;
			var moddedValue = modded.Flags;
			var vanillaValues = vanillas.Select(x => x.Flags);

			patched.Flags = patchedValue.PatchRevertedFlags(vanillaValues, moddedValue, ref changed);
		}

		#endregion
		#region Placed object specific
		public static void PatchPrimitive(this PlacedObject patched, HashSet<IPlacedObjectGetter> vanillas, IPlacedObjectGetter modded, ref bool changed)
		{
			var patchedValue = patched.Primitive;
			var moddedValue = modded.Primitive;
			var vanillaValues = vanillas.Select(x => x.Primitive);

			var oldPatchedBounds = patched.Primitive?.Bounds;
			var moddedBounds = modded.Primitive?.Bounds;
			var vanillaBounds = vanillas.Select(x => x.Primitive?.Bounds);
			bool boundsChanged = false;
			var newPatchedBounds = oldPatchedBounds.PatchReverted(vanillaBounds, moddedBounds, ref boundsChanged, 1E-04f);

			if (boundsChanged)
			{
				changed = true;
				if (patchedValue == null || moddedValue == null)
					patched.Primitive = moddedValue?.DeepCopy();
				else
					patchedValue.Bounds = newPatchedBounds!.Value;
			}
		}
		public static void PatchLightData(this PlacedObject patched, HashSet<IPlacedObjectGetter> vanillas, IPlacedObjectGetter modded, ref bool changed)
		{
			var patchedValue = patched.LightData;
			var moddedValue = modded.LightData;
			var vanillaValues = vanillas.Select(x => x.LightData);

			bool membersgRevertedToVanilla = vanillaValues.Any(x =>
			{
				var reverted = new bool[5];
				var isVanilla = new bool[4];

				reverted[0] = Util.RevertedToVanilla(patchedValue?.FovOffset, x?.FovOffset, moddedValue?.FovOffset, ref isVanilla[0]);
				reverted[1] = Util.RevertedToVanilla(patchedValue?.FadeOffset, x?.FadeOffset, moddedValue?.FadeOffset, ref isVanilla[1]);
				reverted[2] = Util.RevertedToVanilla(patchedValue?.ShadowDepthBias, x?.ShadowDepthBias, moddedValue?.ShadowDepthBias, ref isVanilla[2]);
				reverted[3] = Util.RevertedToVanilla(patchedValue?.Versioning, x?.Versioning, moddedValue?.Versioning, ref isVanilla[3]);

				// Only check unknown if versioning matches
				if (isVanilla[3])
					reverted[4] = Util.RevertedToVanilla(patchedValue?.Unknown, x?.Unknown, moddedValue?.Unknown, ref dummyBool);

				return isVanilla.All(x => x) && reverted.Any(x => x);
			});

			if (membersgRevertedToVanilla)
			{
				changed = true;
				patched.LightData = moddedValue?.DeepCopy();
			}
		}
		#endregion
	}
}
