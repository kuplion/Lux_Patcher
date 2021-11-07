// Custom made, some cells from v4 that were deleted in v5

using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public static partial class LegacyoftheDragonborn
    {
        public static class CellLegacy
        {
            private static FormLink<ICellGetter> Construct(uint id) => new FormLink<ICellGetter>(ModKey.MakeFormKey(id));
            public static FormLink<ICellGetter> DBMDGAetheriumRoom => Construct(0x5925);
            public static FormLink<ICellGetter> DBMDGArmoryLower => Construct(0x9F18F);
            public static FormLink<ICellGetter> DBMDGArmoryWest => Construct(0x167100);
            public static FormLink<ICellGetter> DBMDGArmourySouth => Construct(0x167A37);
            public static FormLink<ICellGetter> DBMDGBookStacks => Construct(0x167C99);
            public static FormLink<ICellGetter> DBMDGCultureandArt => Construct(0x8F3381);
            public static FormLink<ICellGetter> DBMDGDaedricHall => Construct(0x1263AD);
            public static FormLink<ICellGetter> DBMDGHallofHeroesHeist => Construct(0x3DC91);
            public static FormLink<ICellGetter> DBMDGHallofLegends => Construct(0x7095C2);
            public static FormLink<ICellGetter> DBMDGHallofOddities => Construct(0x16849A);
            public static FormLink<ICellGetter> DBMDGNaturalScienceHaunted => Construct(0x7C0D41);
            public static FormLink<ICellGetter> DBMDGUpperGallery => Construct(0x8CF13D);
            public static FormLink<ICellGetter> DBMDGWineCellar => Construct(0x168B05);
        }
    }
}
