using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace ELE_Patcher.Utility
{
	public static class Modded
	{
		#region Mod identifier constants, to avoid typos
		public static readonly ModKey KeyBruma = ModKey.FromNameAndExtension("BSHeartland.esm");
		public static readonly ModKey KeyCRF = ModKey.FromNameAndExtension("Cutting Room Floor.esp");
		public static readonly ModKey KeyDarkend = ModKey.FromNameAndExtension("Darkend.esp");
		public static readonly ModKey KeyFalskaar = ModKey.FromNameAndExtension("Falskaar.esm");
		public static readonly ModKey KeyHelgenReborn = ModKey.FromNameAndExtension("Helgen Reborn.esp");
		public static readonly ModKey KeyLanterns = ModKey.FromNameAndExtension("Lanterns Of Skyrim - All In One - Main.esm");
		public static readonly ModKey KeyLotDb = ModKey.FromNameAndExtension("LegacyoftheDragonborn.esm");
		public static readonly ModKey KeyMedievalLanterns = ModKey.FromNameAndExtension("Medieval Lanterns of Skyrim.esp");
		public static readonly ModKey KeyRavengate = ModKey.FromNameAndExtension("Ravengate.esp");
		#endregion
		#region ELE-related constants
		private static readonly Color candle = Color.FromArgb(220, 160, 90);
		private static readonly Color fire = Color.FromArgb(255, 160, 64);
		#endregion
		#region ELE links
		// Lighting templates
		private static readonly FormLink<ILightingTemplateGetter> brightLt = ELE_SSE.LightingTemplate._1ELE_Bright_LT;
		private static readonly FormLink<ILightingTemplateGetter> brighterLt = ELE_SSE.LightingTemplate._1ELE_Brighter_LT;
		private static readonly FormLink<ILightingTemplateGetter> darkLt = ELE_SSE.LightingTemplate._1ELE_Dark_LT;
		private static readonly FormLink<ILightingTemplateGetter> darkDwemerLt = ELE_SSE.LightingTemplate._1ELE_Dark_Dwemer_LT;
		private static readonly FormLink<ILightingTemplateGetter> darkFalmerLt = ELE_SSE.LightingTemplate._1ELE_Dark_Falmer_LT;
		private static readonly FormLink<ILightingTemplateGetter> darkIceLt = ELE_SSE.LightingTemplate._1ELE_Dark_Ice_LT;
		private static readonly FormLink<ILightingTemplateGetter> darkThinFogLt = ELE_SSE.LightingTemplate._1ELE_Dark_ThinFog_LT;
		private static readonly FormLink<ILightingTemplateGetter> mediumLt = ELE_SSE.LightingTemplate._1ELE_Medium_LT;

		// Image spaces
		private static readonly FormLink<IImageSpaceGetter> dungeonIs = ELE_SSE.ImageSpace._1ELE_Dungeon_IS;
		private static readonly FormLink<IImageSpaceGetter> interiorIs = ELE_SSE.ImageSpace._1ELE_Interior_IS;

		// Combinations, to save writing, lighting templates mostly come with the same image space
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) bright = (brightLt, interiorIs);
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) brighter = (brighterLt, interiorIs);
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) dark = (darkLt, dungeonIs);
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) darkDwemer = (darkDwemerLt, dungeonIs);
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) darkFalmer = (darkFalmerLt, dungeonIs);
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) darkIce = (darkIceLt, dungeonIs);
		private static readonly (FormLink<ILightingTemplateGetter>, FormLink<IImageSpaceGetter>) medium = (mediumLt, dungeonIs);

		private static readonly (FormLink<ILightingTemplateGetter>?, FormLink<IImageSpaceGetter>) dungeonOnly = (null, dungeonIs);
		#endregion

		private static Dictionary<string, FormLink<IImageSpaceGetter>> GenerateCustomImageSpaces(this HashSet<ModKey> mods, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
		{
			Dictionary<string, FormLink<IImageSpaceGetter>> dictionary = new();

			#region Beyond Skyrim - Bruma
			if (mods.Contains(KeyBruma))
			{
				// Create 1ELE_DungeonAyleid_IS
				var ayleidDungeonEdid = "ELE_Patcher_1ELE_DungeonAyleid_IS";
				var eleDungeonIs = dungeonIs.Resolve(state.LinkCache);
				var ayleidDungeonIs = state.PatchMod.ImageSpaces.DuplicateInAsNewRecord(eleDungeonIs, ayleidDungeonEdid);
				ayleidDungeonIs.EditorID = ayleidDungeonEdid;

				if (ayleidDungeonIs.Hdr != null)
				{
					ayleidDungeonIs.Hdr.BloomThreshold = 0.5f;
					ayleidDungeonIs.Hdr.BloomScale = 1.6f;
					ayleidDungeonIs.Hdr.ReceiveBloomThreshold = 1f;
				}

				if (ayleidDungeonIs.Cinematic != null)
				{
					ayleidDungeonIs.Cinematic.Saturation = 0.8f;
					ayleidDungeonIs.Cinematic.Brightness = 1.0001f;
				}

				if (ayleidDungeonIs.Tint != null)
					ayleidDungeonIs.Tint.Color = System.Drawing.Color.FromArgb(128, 198, 255);

				if (ayleidDungeonIs.DepthOfField != null)
				{
					ayleidDungeonIs.DepthOfField.Strength = 0.6f;
					ayleidDungeonIs.DepthOfField.Distance = 500f;
					ayleidDungeonIs.DepthOfField.Range = 1500f;
					ayleidDungeonIs.DepthOfField.Sky = false;
					ayleidDungeonIs.DepthOfField.BlurRadius = 0;
				}

				dictionary.Add(ayleidDungeonEdid, new(ayleidDungeonIs.FormKey));
			}
			#endregion

			return dictionary;
		}

		#region Add record info from mods
		// All info is from ELE's official patch unless said otherwise
		public static Dictionary<FormLink<ICellGetter>, (FormLink<ILightingTemplateGetter>? lightingTemplate, FormLink<IImageSpaceGetter>? imageSpace)> GetCellInfo(this HashSet<ModKey> mods, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
		{
			var customImageSpaces = mods.GenerateCustomImageSpaces(state);

			Dictionary<FormLink<ICellGetter>, (FormLink<ILightingTemplateGetter>?, FormLink<IImageSpaceGetter>?)> dictionary = new();

			#region Beyond Skyrim - Bruma
			if (mods.Contains(KeyBruma))
			{
				var ayleidDungeonIs = customImageSpaces.GetValueOrDefault("ELE_Patcher_1ELE_DungeonAyleid_IS");

				dictionary.Add(BSHeartland.Cell.CYRAnga, (darkDwemerLt, ayleidDungeonIs));
				dictionary.Add(BSHeartland.Cell.CYRApplewatchAebondsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRApplewatchBenuniAlfenasHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRApplewatchGergusMalumeasHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBorealStoneCave, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRBrumaACutAbove, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaAlbeciusJucanisHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaAnanrilsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaBasementAlbecius, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaBasementPlayerHouse, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaBasementRenod, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaBasementSellus, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaBotramtheHammersHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCastleBarracks, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCastleDungeon, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCastleGreatHall, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCastleGuestWing, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCastleLordsManor, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCastleServiceHall, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCathedralofStMartin, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCaverns, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRBrumaChapelHall, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaChapelUndercroft, dark);
				dictionary.Add(BSHeartland.Cell.CYRBrumaCondemnedHouse, (mediumLt, interiorIs));
				dictionary.Add(BSHeartland.Cell.CYRBrumaFightersGuild, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaFightersGuildBasement, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaGalarynnsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaGryfardPetonsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaHaraldBurdssonsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaIceWindTraders, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaInterior03, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaJerallViewBath, dark);
				dictionary.Add(BSHeartland.Cell.CYRBrumaJerallViewInn, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaJerallViewInnBasement, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaNorthernArms, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaRenodsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaSellusPreliusHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack01LeoAndBaliusHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack02TedralsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack03, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack04ArilusHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack05RiljasHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack06, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaShack07EdwarinsHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaSilverPlowHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaStenarCenosHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaSynodBasement, medium);
				dictionary.Add(BSHeartland.Cell.CYRBrumaSynodConclave, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaTheRestfulWatchman, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaWhitePineLodge, bright);
				dictionary.Add(BSHeartland.Cell.CYRBrumaWildeyeStables, bright);
				dictionary.Add(BSHeartland.Cell.CYRCapstoneCave, dark);
				dictionary.Add(BSHeartland.Cell.CYRCloudRulerTemple, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRCLoudRulerTempleArmory, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRCloudRulerTempleEastWing, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRCloudRulerTempleWestWing, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRFortCutpurse, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortCutpurseJail, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortCutpurseTower, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortHorunn01, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortHorunn02, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortHorunn03, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortPalePass, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortPalePassCaptainsQuarters, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortPalePassPrison, medium);
				dictionary.Add(BSHeartland.Cell.CYRFortPalePassSubstructure, medium);
				dictionary.Add(BSHeartland.Cell.CYRFreezewindHollow, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRFrostCragSpire, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRFrostCragSpireTower, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRFrostfireGlade01, dark);
				dictionary.Add(BSHeartland.Cell.CYRFrostfireGlade02, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRFrozenGrotto, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRGautierreManor, bright);
				dictionary.Add(BSHeartland.Cell.CYRGautierreManorBasement, medium);
				dictionary.Add(BSHeartland.Cell.CYRGreenwoodAfersHouse, bright);
				dictionary.Add(BSHeartland.Cell.CYRGreenwoodMeadery, bright);
				dictionary.Add(BSHeartland.Cell.CYRGuttedMine, dark);
				dictionary.Add(BSHeartland.Cell.CYRHjaltisRefuge, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRJerallHuntersShack, bright);
				dictionary.Add(BSHeartland.Cell.CYRJerallHuntersShackDestroyed, medium);
				dictionary.Add(BSHeartland.Cell.CYRLakesideRetreatDUPLICATE001, bright);
				dictionary.Add(BSHeartland.Cell.CYRNorthfringeSanctum01, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRNorthfringeSanctum02, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRPlunderedMine, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRRedRubyCave01, dark);
				dictionary.Add(BSHeartland.Cell.CYRRedRubyCave02, dark);
				dictionary.Add(BSHeartland.Cell.CYRRielle01, (darkIceLt, ayleidDungeonIs));
				dictionary.Add(BSHeartland.Cell.CYRRielle02, (darkLt, ayleidDungeonIs));
				dictionary.Add(BSHeartland.Cell.CYRRiversideShack, bright);
				dictionary.Add(BSHeartland.Cell.CYRRiversideShackBasement, medium);
				dictionary.Add(BSHeartland.Cell.CYRSedorV201, (darkLt, ayleidDungeonIs));
				dictionary.Add(BSHeartland.Cell.CYRSedorV202, (darkIceLt, ayleidDungeonIs));
				dictionary.Add(BSHeartland.Cell.CYRSedorV203, (darkLt, ayleidDungeonIs));
				dictionary.Add(BSHeartland.Cell.CYRSerpentsTrail01, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRSerpentsTrail02, darkIce);
				dictionary.Add(BSHeartland.Cell.CYRSerpentsTrail03, darkIce);
				dictionary.Add(BSHeartland.Cell.CyrSilvertoothCaveV201, dark);
				dictionary.Add(BSHeartland.Cell.CyrSilvertoothCaveV202, dark);
				dictionary.Add(BSHeartland.Cell.CYRSnowstoneRest, dark);
				dictionary.Add(BSHeartland.Cell.CYRTheBeastsMaw, dark);
				dictionary.Add(BSHeartland.Cell.CYRToadstoolHollow, dark);
				dictionary.Add(BSHeartland.Cell.CYRUnderpall01, dark);
				dictionary.Add(BSHeartland.Cell.CYRUnderpall02, dark);
				dictionary.Add(BSHeartland.Cell.CYRUnderpall03, dark);
				dictionary.Add(BSHeartland.Cell.CYRUnderpall04, dark);
				dictionary.Add(BSHeartland.Cell.CYRUnderpall05, dark);
				dictionary.Add(BSHeartland.Cell.CYRUnmarkedCave, dark);

				// Added
				dictionary.Add(BSHeartland.Cell.CYRForesterOutpostForestMountainInterior01, bright);

				#region Ignored for patching
				/* Present in official patch, removed here since unused
				 * CYRAleswellCrextusResidence, CYRAleswellFalegusResidence, CYRAleswellInn, CYRAleswellMathiasEndieriHouse, CYRAleswellUmogDarSeelHouse
				 * CYRCloudTopLookout
				 * CYRCrumblingMine
				 * CYRDirichKeepFarmhouse
				 * CYRDzonotCave
				 * CYRFortCaractacus01
				 * CYRFortColdcorn01, CYRFortColdcorn02
				 * CYRFortHomestead
				 * CYRFortRayles01
				 * CYRFrostCragSpireVault
				 * CyrGarlasAgea, CyrGarlasTure
				 * CYRGnollPass01
				 * CYRLakesideRetreat
				 * CYRLighthouseGoldCoast
				 * CYRMountainWatchAvringErvigHouse, CYRMountainWatchAubertHouse, CYRMountainWatchBunkhouse, CYRMountainWatchZurikkiHouse
				 * CYRNorthfringeSanctum03, CYRNorthfringeSanctum04, CYRNorthfringeSanctum05
				 * CYROutlawEndreCave
				 * CYRPellsGateInterior01BlacielAnHouse
				 * CYRSedor
				 * CyrSidewaysCave01, CyrSidewaysCave02, CyrSidewaysCave03
				 * CYRWatersEdgeInterior06
				 * CYRWellappBanditMine
				 * CYRWeyeBastienAdrardHouse, CYRWeyeDominitianFarm, CYRWeyeDurashGraAgumHouse, CYRWeyeGeneralStore, CYRWeyeGrennHouse, CYRWeyeMessalaFarm, CYRWeyeWarehouseAux
				 */
				#endregion
			}
			#endregion
			#region Cutting Room Floor
			if (mods.Contains(KeyCRF))
			{
				// Note: Official patch doesn't change inherit flags at all, probably an oversight
				dictionary.Add(CuttingRoomFloor.Cell.CRFBarleydarkFarmInterior, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFFrostRiverHenrikHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFFrostRiverMeadery, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFFrostRiverRogensHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFHeljarchenApothecary, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFHeljarchenBlacksmith, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFHeljarchenHeigensHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFHeljarchenJensHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFIrontreeMillTrilfsHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFStonehillsAleucHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFStonehillsArgiFarseersHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFStonehillsTalibHouse, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFWhiterunMaidenLoomManor, bright);
				dictionary.Add(CuttingRoomFloor.Cell.CRFWhiterunWintersandManorHouse, bright);

				// Added
				dictionary.Add(CuttingRoomFloor.Cell.CRFStonehillsGesturHouse, bright);
			}
			#endregion
			#region Darkend
			if (mods.Contains(KeyDarkend))
			{
				dictionary.Add(Darkend.Cell.XJKAbandonedMansionInterior, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKAncientTowerDeepDeeps, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKAncientTowerDeepDeepsLAVA, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKAncientTowerDeeps, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKAncientTowerDeeps03, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKcastleBasement, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKcastleInteriorFIRSTfloor, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKcastleInteriorGROUNDfloor, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKcastleInteriorLIBRARYfloor, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKcastleInteriorSECONDfloor, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKcastleInteriorTOWERstairs, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKFarmInteriors, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKfortress, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKFortSmallinterior, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKMine, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKphalosChurchCatacombs, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKphalosCurch, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKphalosPortWarehouse, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKphalosShipsInteriors, dungeonOnly);
				dictionary.Add(Darkend.Cell.XJKSwampCatacombs, dungeonOnly);
			}
			#endregion
			#region Falskaar
			if (mods.Contains(KeyFalskaar))
			{
				dictionary.Add(Falskaar.Cell.FSAmberCreekAmberMeadInn, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekBarracks, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekBorvaldurManor, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekGeneralStore, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekKunnarisFarm, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekMine, dark);
				dictionary.Add(Falskaar.Cell.FSAmberCreekMineQuarters, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekOudinsHouse, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekPlayerHome, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekRangarrsHouse, bright);
				dictionary.Add(Falskaar.Cell.FSAmberCreekRuriksHouse, bright);
				dictionary.Add(Falskaar.Cell.FSAspenfallLodge, bright);
				dictionary.Add(Falskaar.Cell.FSAudmundsFarm, bright);
				dictionary.Add(Falskaar.Cell.FSBailunPriory, bright);
				dictionary.Add(Falskaar.Cell.FSBearclawCave, dark);
				dictionary.Add(Falskaar.Cell.FSBjarriksDemise, darkIce);
				dictionary.Add(Falskaar.Cell.FSBorvaldCatacombs, dark);
				dictionary.Add(Falskaar.Cell.FSBorvaldValfredManor, bright);
				dictionary.Add(Falskaar.Cell.FSBrittlerunCave, dark);
				dictionary.Add(Falskaar.Cell.FSDocksWatersEdgeTrader, bright);
				dictionary.Add(Falskaar.Cell.FSDocksWulfsHouse, bright);
				dictionary.Add(Falskaar.Cell.FSEchoDeepMine, dark);
				dictionary.Add(Falskaar.Cell.FSFalskaarLighthouse, bright);
				dictionary.Add(Falskaar.Cell.FSFortUrokk, dark);
				dictionary.Add(Falskaar.Cell.FSGrimrottGrotto, dark);
				dictionary.Add(Falskaar.Cell.FSHjalmarArmory01, bright);
				dictionary.Add(Falskaar.Cell.FSHjalmarArmory02, bright);
				dictionary.Add(Falskaar.Cell.FSHjorgunnarManor01, bright);
				dictionary.Add(Falskaar.Cell.FSHjorgunnarManor02, bright);
				dictionary.Add(Falskaar.Cell.FSKalrunMonastery, bright);
				dictionary.Add(Falskaar.Cell.FSMammothKeep, dark);
				dictionary.Add(Falskaar.Cell.FSMountainMistTemple, dark);
				dictionary.Add(Falskaar.Cell.FSMzubthand, darkDwemer);
				dictionary.Add(Falskaar.Cell.FSNorthernPass, dungeonOnly);
				dictionary.Add(Falskaar.Cell.FSPinecastleGrove, dark);
				dictionary.Add(Falskaar.Cell.FSPinevaleMine, dark);
				dictionary.Add(Falskaar.Cell.FSReinaldurFarmstead, dark);
				dictionary.Add(Falskaar.Cell.FSRiverwatchHotSprings, dark);
				dictionary.Add(Falskaar.Cell.FSRuinsOfHolmr, dark);
				dictionary.Add(Falskaar.Cell.FSSandyshellHollow, dark);
				dictionary.Add(Falskaar.Cell.FSShatteredAxeRedoubt, dark);
				dictionary.Add(Falskaar.Cell.FSStoneridgeWatch01, dark);
				dictionary.Add(Falskaar.Cell.FSStoneridgeWatch02, dark);
				dictionary.Add(Falskaar.Cell.FSSunkenSkullBarrow, dark);
				dictionary.Add(Falskaar.Cell.FSUnnvaldrKeep, dark);
				dictionary.Add(Falskaar.Cell.FSVernanHideout, dark);
				dictionary.Add(Falskaar.Cell.FSVolkrundKeep, dark);
				dictionary.Add(Falskaar.Cell.FSWarmthsEdgeCaverns, dark);
				dictionary.Add(Falskaar.Cell.FSWatervineChasm01, dark);
				dictionary.Add(Falskaar.Cell.FSWatervineChasm02, dark);
				dictionary.Add(Falskaar.Cell.FSWatervineChasm03, dark);

				#region Ignored for patching
				/* Present in official patch, removed here since unused
				 * FSIceFort
				 * FSStargazerGrove
				 */
				#endregion
			}
			#endregion
			#region Helgen Reborn
			if (mods.Contains(KeyHelgenReborn))
			{
				dictionary.Add(HelgenReborn.Cell.aaaBalokBanditCrypt01, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokBanditMine, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokBrothel, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokDraugrCrypt, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokDraugrCrypt02, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokFightCave, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHamingsHouse, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHelgenInn, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouse01, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouse02, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouse03, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouse04, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouse05, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouseHill01, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouseHill02, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouseHill03, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouseHill04, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokHouseHill05, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokMine01, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokNecroLair, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokOrphansCave, darkIce);
				dictionary.Add(HelgenReborn.Cell.aaaBalokReinhardtInterior, bright);
				dictionary.Add(HelgenReborn.Cell.aaaBalokThalmorPrison, (darkLt, interiorIs));
				dictionary.Add(HelgenReborn.Cell.aaaBalokThalmorPrison02, (darkLt, interiorIs));
				dictionary.Add(HelgenReborn.Cell.aaaBalokTower, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokTowerDisplay, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokTowerLower, dark);
				dictionary.Add(HelgenReborn.Cell.aaaBalokVampLair, dark);

				#region Ignored for patching
				/* Present in official patch, removed here since unused
				 * aaaBalokAerandilShip
				 * aaaBalokCrypt01
				 * aaaBalokDoorRoom
				 */
				#endregion
			}
			#endregion
			#region Legacy of the Dragonborn
			if (mods.Contains(KeyLotDb))
			{
				// Check version
				bool old = !LegacyoftheDragonborn.Cell.DBMDGArmoryWest.TryResolve(state.LinkCache, out _);

				dictionary.Add(LegacyoftheDragonborn.Cell.SancreTorCatacombs, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.SancreTorHallOfJudgement, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.SancreTorMainHall, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.SancreTorPrisons, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.SancreTorSealedHalls, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.SancreTorTiberSeptimShrine, dark);

				dictionary.Add(LegacyoftheDragonborn.Cell.DBMAirshipCabin, bright);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMAyleidPocketRealm, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMDeepholme, dark);

				dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGPlanetarium, medium); // Fine due to light 0x18F8DB in the center of the room

				dictionary.Add(LegacyoftheDragonborn.Cell.DBMDragonsFall, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMDragonsFallDepths, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMDragonsFallSanctum, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMEngelmannsRest, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMEngelmannsSanctuary, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFieldStation01, bright);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFieldStation02, bright);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFieldStation03, bright);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFortPalePass, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFortPalePassStoreroom, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFortPalePassVaults, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMFortPalePassWestTower, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMGreenwallRuins, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMJolgeirrBarrow, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMJournalDungeon01, darkDwemer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMMAASEAftlandAnnex, darkDwemer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMMAASEIrkDescent, darkDwemer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMMAASEIrkRefuge, darkDwemer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMMAASEXribAnnex, darkDwemer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMPalePassPassage, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRavenRest, dark);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRkund01, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRkund02, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRkund03, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRkund04, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRkund05, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRkund06, darkDwemer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMSagruunde, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMSanctum, (darkThinFogLt, dungeonIs));
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMSnowElfTemple, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMTrinimacTemple01, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMTrinimacTemple02, darkFalmer);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMWindcallerPassN, darkIce);
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMWindcallerPassS, darkIce);

				// v5 overhauled the museum, relying heavily on lighting templates, while v4 actually still used lighting
				if (old)
				{
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGAetheriumRoom, darkDwemer);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGArmory, dungeonOnly);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGArmoryEast, darkDwemer);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGArmoryLower, darkDwemer);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGArmoryWest, darkDwemer);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGArmourySouth, darkDwemer);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGBookStacks, dark);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGCultureandArt, medium);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGDaedricHall, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGDragonbornHall, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofHeroes, medium);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGHallofHeroesHeist, medium);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGHallofLegends, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGEastExhibitHalls, medium); // Used to be DBMDGHallofLostEmpires
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGHallofOddities, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofSecrets, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGLibrary, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGNaturalScience, medium);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGNaturalScienceHaunted, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGSafehouse, medium);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGStoreroom, medium);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGUpperGallery, medium);
					dictionary.Add(LegacyoftheDragonborn.CellLegacy.DBMDGWineCellar, dark);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMGuildhouse, bright);
				}
				else
				{
					// Use brighter templates for v5
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGArmory, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGArmoryEast, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGArmoryWest, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGDragonbornHall, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofHeroes, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofHeroesHeist, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofHeroesHaunted, (brighterLt, null));
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGNaturalScience, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGNaturalScienceHaunted, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofSecrets, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGHallofSecretsBare, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGLibrary, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGSafehouse, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGEastExhibitHalls, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMGuildhouse, brighter);
					dictionary.Add(LegacyoftheDragonborn.Cell.DBMDGStoreroom, brighter);
				}

				// Added
				dictionary.Add(LegacyoftheDragonborn.Cell.DBMRainsShelter, bright);
				dictionary.Add(LegacyoftheDragonborn.Cell.DLC2MoesringPass02DBM, dark);
			}
			#endregion
			#region Ravengate
			if (mods.Contains(KeyRavengate))
			{
				// Note: Official patch doesn't change inherit flags at all, probably an oversight
				dictionary.Add(Ravengate.Cell.RVHCellManor, dark);
				dictionary.Add(Ravengate.Cell.RVHCellOblivion, dungeonOnly);
				dictionary.Add(Ravengate.Cell.RVHCellRatway, dark);
				dictionary.Add(Ravengate.Cell.RVHCellRatwayDepths, dark);
			}
			#endregion

			return dictionary;
		}
		public static Dictionary<FormLink<IImageSpaceGetter>, (float? brightness, float? contrast)> GetImageSpaceInfo(this HashSet<ModKey> mods)
		{
			Dictionary<FormLink<IImageSpaceGetter>, (float?, float?)> dictionary = new();

			#region Legacy of the Dragonborn
			if (mods.Contains(KeyLotDb))
				dictionary.Add(LegacyoftheDragonborn.ImageSpace.DBM_HauntedMuseumIMG, (1.0001f, 1.3f));
			#endregion

			return dictionary;
		}
		public static Dictionary<FormLink<ILightGetter>, Color> GetLightsInfo(this HashSet<ModKey> mods)
		{
			Dictionary<FormLink<ILightGetter>, Color> dictionary = new();

			#region Beyond Skyrim - Bruma
			if (mods.Contains(KeyBruma))
			{
				Color ayleidWhite = Color.FromArgb(152, 196, 231); // A pale blue

				dictionary.Add(BSHeartland.Light.CYRAyleidWhiteLight, ayleidWhite);
				dictionary.Add(BSHeartland.Light.CYRAyleidWhiteLightShadow01, ayleidWhite);
				dictionary.Add(BSHeartland.Light.CYRAyleidWhitePulsatingLight, ayleidWhite);
				dictionary.Add(BSHeartland.Light.CYRBrumaLightFireNS, fire);
				dictionary.Add(BSHeartland.Light.CYRBrumaLightFireShadow, fire);
				dictionary.Add(BSHeartland.Light.CYRCathedralCandleLight01, candle);
				dictionary.Add(BSHeartland.Light.CYRCathedralCandleLight01NS, candle);
				dictionary.Add(BSHeartland.Light.CYRdunBeastsMawLightCampFireShadow, fire);
				dictionary.Add(BSHeartland.Light.CYRLightCampFireNS, fire);
				dictionary.Add(BSHeartland.Light.CYRLightCampFireShadow, fire);
				dictionary.Add(BSHeartland.Light.CYRLightCandleAleswell01NS, candle);
				dictionary.Add(BSHeartland.Light.CYRLightCandleAleswell02NS, candle);
				dictionary.Add(BSHeartland.Light.CYRLightCandleStreet01NS, candle);
				dictionary.Add(BSHeartland.Light.CYRLightCandleStreet01Shadow, candle);
				dictionary.Add(BSHeartland.Light.CYRLightForgeFire01Interior, fire);

				// Added
				dictionary.Add(BSHeartland.Light.CYRAyleidWhiteLightShadow02, ayleidWhite);
			}
			#endregion
			#region Darkend
			if (mods.Contains(KeyDarkend))
				dictionary.Add(Darkend.Light.XJKTorch01Shadow256, fire);
			#endregion
			#region Helgen Reborn
			if (mods.Contains(KeyHelgenReborn))
			{
				// Added
				dictionary.Add(HelgenReborn.Light.BalokBrazierLightsCavern, fire);
			}
			#endregion
			#region Lanterns of Skyrim
			if (mods.Contains(KeyLanterns))
				dictionary.Add(LanternsOfSkyrim.Light.mannyLanternsOfSkyrimAllInOne, candle);
			#endregion
			#region Legacy of the Dragonborn
			if (mods.Contains(KeyLotDb))
			{
				dictionary.Add(LegacyoftheDragonborn.Light.DBM_Torch, fire);
				dictionary.Add(LegacyoftheDragonborn.Light.DBM_TorchREPLICA, fire);	
			}
			#endregion
			#region Medieval Lanterns of Skyrim
			if (mods.Contains(KeyMedievalLanterns))
				dictionary.Add(MedievalLanternsOfSkyrim.Light.MLOS_candlelight_01, candle);
			#endregion
			#region Ravengate
			if (mods.Contains(KeyRavengate))
			{
				// Added
				dictionary.Add(Ravengate.Light.RVH_Light_BurnItDown, fire);
				dictionary.Add(Ravengate.Light.RVH_Light_DeathFire, fire);
				dictionary.Add(Ravengate.Light.RVH_Light_DefaultCandleLight01NSDesat_Bright, candle);
				dictionary.Add(Ravengate.Light.RVH_Light_DefaultCandleLight01NS_Bright, candle);
				dictionary.Add(Ravengate.Light.RVH_Light_DefaultCandleLight01NS_Loraliah, candle);
				dictionary.Add(Ravengate.Light.RVH_Light_DefaultCandleLight01NS_Saturated, candle);
				dictionary.Add(Ravengate.Light.RVH_Light_DefaultCandleLight01NS_Saturated_Hemi, candle);
				dictionary.Add(Ravengate.Light.RVH_Light_DefaultTorch01NS_Bright, fire);
				dictionary.Add(Ravengate.Light.RVH_Light_LightCampFire01_Arena, fire);
				dictionary.Add(Ravengate.Light.RVH_Light_LightCampFire01_Shadows, fire);
				dictionary.Add(Ravengate.Light.RVH_Light_Oblivion_Fire, fire);
			}
			#endregion

			return dictionary;
		}
		#endregion

		#region Set fields to new values
		// Provided value being null means it's not set
		public static void PatchInherits(this Cell patched, CellLighting.Inherit? newValue, ref bool changed)
		{
			var patchedValue = patched.Lighting?.Inherits;

			if (newValue == null || patchedValue == null || patchedValue.Equals(newValue))
				return;

			changed = true;
			patched.Lighting!.Inherits = (CellLighting.Inherit)newValue;
		}
		public static void PatchLightingTemplate(this Cell patched, FormLink<ILightingTemplateGetter>? newValue, ref bool changed)
		{
			var patchedValue = patched.LightingTemplate;

			if (newValue == null || patchedValue.Equals(newValue))
				return;

			changed = true;
			patched.LightingTemplate = newValue;

			// Also patch flags. If flags are something else, add to to PatchCellsExtra
			if (patched.Lighting != null)
				patched.Lighting.Inherits |= (CellLighting.Inherit)2047;
		}
		public static void PatchImageSpace(this Cell patched, FormLink<IImageSpaceGetter>? newValue, ref bool changed)
		{
			var patchedValue = patched.ImageSpace;

			if (newValue == null || patchedValue.FormKey.Equals(newValue.FormKey))
				return;

			changed = true;
			patched.ImageSpace.SetTo(newValue.FormKey);

			patched.ImageSpace = newValue.AsNullable();
		}

		public static void PatchCinematic(this ImageSpace patched, (float? brightness, float? contrast) newValues, ref bool changed)
		{
			var patchedCinematic = patched.Cinematic;

			if (patchedCinematic == null)
				return;

			if (newValues.brightness != null && !Util.NearlyEquals(patchedCinematic.Brightness, newValues.brightness))
			{
				changed = true;
				patchedCinematic.Brightness = (float)newValues.brightness;
			}

			if (newValues.contrast != null && !Util.NearlyEquals(patchedCinematic.Contrast, newValues.contrast))
			{
				changed = true;
				patchedCinematic.Contrast = (float)newValues.contrast;
			}
		}

		public static void PatchColor(this Light patched, Color newValue, ref bool changed)
		{
			var patchedValue = patched.Color;

			if (patchedValue.Equals(newValue))
				return;

			changed = true;
			patched.Color = newValue;
		}
		#endregion
		#region Patch extra stuff
		// Most patches only change a handful of values, any value changes other than that go here
		public static void PatchCellsExtra (this HashSet<ModKey> mods, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
		{
			#region Beyond Skyrim - Bruma
			if (mods.Contains(KeyBruma))
			{
				// Inherit flags for Unmarked cave
				if (BSHeartland.Cell.CYRUnmarkedCave.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache, out var caveContext))
				{
					var cave = caveContext.Record.DeepCopy();

					var unwantedInherits = CellLighting.Inherit.FogColor;
					if (cave.Lighting != null && cave.Lighting.Inherits.HasAnyFlag(unwantedInherits))
					{
						cave.Lighting.Inherits &= ~unwantedInherits;
						caveContext
							.GetOrAddAsOverride(state.PatchMod)
							.DeepCopyIn(cave, new Cell.TranslationMask(false) { Lighting = true });
					}
				}
			}
			#endregion

			if (mods.Contains(KeyFalskaar))
			{
				// Inherit flags for Bjarrik's demise & Borval catacombs
				if (Falskaar.Cell.FSBjarriksDemise.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache, out var bjarrikContext))
				{
					var bjarrik = bjarrikContext.Record.DeepCopy();

					var unwantedInherits = CellLighting.Inherit.FogNear | CellLighting.Inherit.FogFar | CellLighting.Inherit.FogPower;
					if (bjarrik.Lighting != null && bjarrik.Lighting.Inherits.HasAnyFlag(unwantedInherits))
					{
						bjarrik.Lighting.Inherits &= ~unwantedInherits;
						bjarrikContext
							.GetOrAddAsOverride(state.PatchMod)
							.DeepCopyIn(bjarrik, new Cell.TranslationMask(false) { Lighting = true });
					}
				}
				if (Falskaar.Cell.FSBorvaldCatacombs.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache, out var catacombsContext))
				{
					var catacombs = catacombsContext.Record.DeepCopy();

					var unwantedInherits = CellLighting.Inherit.DirectionalColor;
					if (catacombs.Lighting != null && catacombs.Lighting.Inherits.HasAnyFlag(unwantedInherits))
					{
						catacombs.Lighting.Inherits &= ~unwantedInherits;
						catacombsContext
							.GetOrAddAsOverride(state.PatchMod)
							.DeepCopyIn(catacombs, new Cell.TranslationMask(false) { Lighting = true });
					}
				}
			}

			if (mods.Contains(KeyLotDb))
			{
				// Fog clip distance & inherits flags for Ayleid pocket realm
				if (LegacyoftheDragonborn.Cell.DBMAyleidPocketRealm.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache, out var ayleidRealmContext))
				{
					var ayleidRealm = ayleidRealmContext.Record.DeepCopy();
					bool changed = false;

					if (ayleidRealm.Lighting != null)
					{
						if (!Util.NearlyEquals(ayleidRealm.Lighting.FogClipDistance, 5000f))
						{
							changed = true;
							ayleidRealm.Lighting.FogClipDistance = 5000f;
						}

						var unwantedInherits = CellLighting.Inherit.ClipDistance | CellLighting.Inherit.LightFadeDistances;
						if (ayleidRealm.Lighting.Inherits.HasAnyFlag(unwantedInherits))
						{
							changed = true;
							ayleidRealm.Lighting.Inherits &= ~unwantedInherits;
						}

						if (changed)
							ayleidRealmContext
								.GetOrAddAsOverride(state.PatchMod)
								.DeepCopyIn(ayleidRealm, new Cell.TranslationMask(false) { Lighting = true });
					}
				}

				// Fog near for Dregas Volar's sanctuary
				if (LegacyoftheDragonborn.Cell.DBMDregasVolarInt.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache, out var dregasContext)) {
					var dregas = dregasContext.Record.DeepCopy();

					if (dregas.Lighting != null && !Util.NearlyEquals(dregas.Lighting.FogNear, 0f))
					{
						dregas.Lighting.FogNear = 0f;
						dregasContext
							.GetOrAddAsOverride(state.PatchMod)
							.DeepCopyIn(dregas, new Cell.TranslationMask(false) { Lighting = true });
					}
				}
			}
		}
		public static void PatchLightsExtra (this HashSet<ModKey> mods, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
		{
			if (mods.Contains(KeyDarkend))
			{
				// Shadowcasting & fade value for the added torch
				var torch = Darkend.Light.XJKTorch01Shadow256.Resolve(state.LinkCache).DeepCopy();
				bool changed = false;

				if (!torch.Flags.HasFlag(Light.Flag.PortalStrict))
				{
					changed = true;
					torch.Flags |= Light.Flag.PortalStrict;
				}

				if (!Util.NearlyEquals(torch.FadeValue, 1.5f))
				{
					changed = true;
					torch.FadeValue = 1.5f;
				}

				if (changed)
					state.PatchMod.Lights.Set(torch);
			}
			if (mods.Contains(KeyLanterns))
			{
				// Fade value for the lantern
				var lantern = LanternsOfSkyrim.Light.mannyLanternsOfSkyrimAllInOne.Resolve(state.LinkCache).DeepCopy();

				if (!Util.NearlyEquals(lantern.FadeValue, 1f))
				{
					lantern.FadeValue = 1f;
					state.PatchMod.Lights.Set(lantern);
				}
			}
		}
		#endregion
	}
}
