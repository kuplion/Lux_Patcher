using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Noggog;
using Loqui;

using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace ELE_Patcher.Utility
{
	public static class Util
	{
		#region General
		public static ISkyrimModDisposableGetter GetModAndMasters(this ModKey key, IPatcherState<ISkyrimMod, ISkyrimModGetter> state, out HashSet<ModKey> masters)
		{
			var mod = GetDisposableMod();
			masters = GetModMasters(mod);
			return mod;

			ISkyrimModDisposableGetter GetDisposableMod()
			{
				var fileName = key.FileName;
				var path = ModPath.FromPath(Path.Combine(state.DataFolderPath, fileName));
				var skyrimVersion = state.GameRelease.ToSkyrimRelease();
				return SkyrimMod.CreateFromBinaryOverlay(path, skyrimVersion);
			}
			HashSet<ModKey> GetModMasters(ISkyrimModDisposableGetter leMod)
			{
				HashSet<ModKey> leMasters = new();
				foreach (var masterRef in leMod.MasterReferences)
					leMasters.Add(masterRef.Master);
				return leMasters;
			}
		}

		/// <summary>
		/// Initializes variables related to the record.<br/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TGetter"></typeparam>
		/// <param name="modRecord"></param>
		/// <param name="state"></param>
		/// <param name="modKey"></param>
		/// <param name="masters"></param>
		/// <param name="vanillaRecords"></param>
		/// <param name="patched"></param>
		/// <param name="changed"></param>
		/// <param name="knownPatches"></param>
		/// <returns>False if record doesn't need to be patched</returns>
		public static bool InitializeRecordVars<T, TGetter>(this TGetter modRecord, IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ModKey modKey, HashSet<ModKey> masters, out HashSet<TGetter> vanillaRecords, [NotNullWhen(true)] out T? patched, out bool changed, HashSet<string>? knownPatches = null)
			where T : SkyrimMajorRecord, TGetter
			where TGetter : class, ISkyrimMajorRecordGetter
		{
			vanillaRecords = new();
			patched = null;
			changed = false;
			var contexts = state.LinkCache.ResolveAllContexts<T, TGetter>(modRecord.FormKey);

			// Don't patch if record originates from modded, or modded is winning override
			var origin = contexts.Last();
			var winner = contexts.First();
			if (origin.ModKey == modKey || winner.ModKey == modKey)
				return false;

			// Don't patch if winner is a known patch to the mod.
			if (knownPatches != null)
			{
				var winnerModFile = winner.ModKey.FileName.String;
				if (knownPatches.Contains(winnerModFile))
					return false;
			}

			// Add vanilla records & patched
			foreach (var context in contexts)
				if (masters.Contains(context.ModKey))
					vanillaRecords.Add(context.Record);

			patched = (T)winner.Record.DeepCopy();
			return true;
		}
		#endregion

		#region Constants
		static bool dummy = false;
		static readonly float twoPI = 2f * MathF.PI;
		public static readonly float rotationAllowedDiff = twoPI / 360E4f;
		#endregion
		#region Normalization
		private static float NormalizeRadians2Pi(this float radians)
		{
			float result = radians % twoPI;
			return result > 0 ? result : result + twoPI;
		}
		internal static P3Float NormalizeRadians2Pi(this P3Float radiansStruct)
		{
			float x = NormalizeRadians2Pi(radiansStruct.X);
			float y = NormalizeRadians2Pi(radiansStruct.Y);
			float z = NormalizeRadians2Pi(radiansStruct.Z);

			return new(x, y, z);
		}
		#endregion
		#region Nearly equals, for floating point numbers
		internal static bool NearlyEquals(float? first, float? second, float allowedDiff = 1E-06f)
		{
			if (first == null || second == null)
				return Equals(first, second);
			else
				return MathF.Abs((float)first - (float)second) < allowedDiff;
		}
		internal static bool NearlyEquals(P3Float? first, P3Float? second, float allowedDiff = 1E-06f)
		{
			bool xEquals = NearlyEquals(first?.X, second?.X, allowedDiff);
			bool yEquals = NearlyEquals(first?.Y, second?.Y, allowedDiff);
			bool zEquals = NearlyEquals(first?.Z, second?.Z, allowedDiff);

			return xEquals && yEquals && zEquals;
		}
		#endregion
		#region Reverted to vanilla
		internal static bool RevertedToVanilla(float? patched, float? vanilla, float? modded, ref bool isVanilla)
		{
			isVanilla = NearlyEquals(patched, vanilla);
			bool isModded = NearlyEquals(patched, modded);

			return !isModded && isVanilla;
		}
		private static bool RevertedToVanilla(P3Float? patched, P3Float? vanilla, P3Float? modded, ref bool isVanilla, float allowedDiff = 1E-06f)
		{
			isVanilla = NearlyEquals(patched, vanilla, allowedDiff);
			bool isModded = NearlyEquals(patched, modded, allowedDiff);

			return !isModded && isVanilla;
		}
		internal static bool RevertedToVanilla<T>(T? patched, T? vanilla, T? modded, ref bool isVanilla)
		{
			isVanilla = Equals(patched, vanilla);
			bool isModded = Equals(patched, modded);

			return !isModded && isVanilla;
		}
		#endregion
		#region Patch reverted
		internal static P3Float? PatchReverted(this P3Float? patched, IEnumerable<P3Float?> vanillas, P3Float? modded, ref bool changed, float allowedDiff = 1E-06f)
		{
			if (vanillas.Any(x => RevertedToVanilla(patched, x, modded, ref dummy, allowedDiff)))
			{
				changed = true;
				return modded;
			}
			else
				return patched;
		}
		internal static int PatchRevertedFlags(this int valueToPatch, IEnumerable<int> vanillaValues, int moddedValue, ref bool changed)
		{
			int valueToPatchCopy = valueToPatch;

			foreach (var vanillaValue in vanillaValues)
				PatchAgainst(ref valueToPatch, vanillaValue, moddedValue);

			if (valueToPatch != valueToPatchCopy)
				changed = true;

			return valueToPatch;

			// Helper function
			void PatchAgainst(ref int flagsToPatch, int vanillaFlags, int moddedFlags)
			{
				int moddedChangedFlags = moddedFlags ^ vanillaFlags;
				int currentChangedFlags = flagsToPatch ^ vanillaFlags;

				// Flip the bits that modded changes but patched doesn't
				flagsToPatch ^= moddedChangedFlags & ~currentChangedFlags;
			}
		}
		internal static TEnum PatchRevertedFlags<TEnum>(this TEnum valueToPatch, IEnumerable<TEnum> vanillaValues, TEnum moddedValue, ref bool changed)
			where TEnum : struct, Enum
		{
			// Convert to int
			int valueToPatchAsInt = Convert.ToInt32(valueToPatch);
			var vanillasAsInt = vanillaValues.Select(x => Convert.ToInt32(x));
			int moddedAsInt = Convert.ToInt32(moddedValue);

			// Patch reverted & convert back to the correct type
			int patchedAsInt = valueToPatchAsInt.PatchRevertedFlags(vanillasAsInt, moddedAsInt, ref changed);
			return (TEnum)Enum.ToObject(typeof(TEnum), patchedAsInt);
		}
		#endregion
		#region Flag helpers
		internal static bool HasAnyFlag(this Enum value, Enum flags)
		{
			var sharedFlags = Convert.ToInt32(value) & Convert.ToInt32(flags);
			return sharedFlags != 0;
		}
		#endregion
	}
}
